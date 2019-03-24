namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    public class MetricService : IMetricService
    {
        private readonly VisionDbContext context;        
        private readonly IMemoryCache cache;

        public MetricService(VisionDbContext context, IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
        }

        public async Task<IEnumerable<MetricItem>> GetCountsAsync()
        {
            const string Counts = "Dashboard.Counts";

            if (!cache.TryGetValue(Counts, out List<MetricItem> counts))
            {
                IEnumerable<Task<MetricItem>> dependencies = AppHelper.DependencyKinds.Select(async kind => new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, kind, $"{kind} dependencies", await context.Dependencies.CountAsync(x => x.Kind == kind)));
                IEnumerable<Task<MetricItem>> assets = AppHelper.DependencyKinds.Select(async kind => new MetricItem(MetricAlertKind.Standard, CategoryKind.Asset, kind, $"{kind} assets", await context.Assets.CountAsync(x => x.Kind == kind)));

                var otherCounts = new MetricItem[]
                {
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.VersionControl, $"Version controls", await context.VersionControls.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Registry, $"Registries", await context.Registries.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.CiCd, $"CICDs", await context.CiCds.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Repository, $"Repositories", await context.Repositories.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Asset, $"Assets", await context.Assets.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Dependencies", await context.Dependencies.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Asset dependencies", await context.AssetDependencies.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Frameworks", await context.Frameworks.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Asset frameworks", await context.AssetFrameworks.CountAsync()),
                };

                counts = otherCounts.Concat(await Task.WhenAll(dependencies.Concat(assets).ToArray())).ToList();

                cache.Set(Counts, counts, DateTimeOffset.Now.AddMinutes(5));
            }

            return counts;
        }

        public async Task<IEnumerable<MetricItem>> GetMetricsAsync(Guid id, CategoryKind targetKind)
        {
            List<MetricItem> items = new List<MetricItem>();

            if (targetKind == CategoryKind.Asset)
            {
                items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Dependencies", await context.AssetDependencies.CountAsync(ad => ad.AssetId == id)));
                items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Frameworks", await context.AssetFrameworks.CountAsync(ad => ad.AssetId == id)));
            }

            if (targetKind == CategoryKind.Dependency)
            {
                items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Assets", await context.AssetDependencies.CountAsync(ad => ad.DependencyId == id)));
                items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Versions", await context.DependencyVersions.CountAsync(ad => ad.DependencyId == id)));

                var dependencies = context.AssetDependencies.Where(ad => ad.DependencyId == id).GroupBy(dv => dv.DependencyVersionId);

                Guid mostId = await dependencies.OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefaultAsync();
                Guid leastId = await dependencies.OrderBy(g => g.Count()).Select(g => g.Key).FirstOrDefaultAsync();

                var most = await context.DependencyVersions.FindAsync(mostId);
                var least = await context.DependencyVersions.FindAsync(leastId);

                var latest = await context.DependencyVersions.FirstOrDefaultAsync(dv => dv.DependencyId == id && dv.IsLatest);

                if (latest != null)
                {
                    items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Latest", latest.Version));
                }

                if (most != null)
                {
                    items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Most used", most.Version));
                }
                
                if (least != null)
                {
                    items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Least used", least.Version));
                }
            }

            if (targetKind == CategoryKind.Repository)
            {
                items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Assets", await context.Assets.CountAsync(ad => ad.RepositoryId == id)));
                
                foreach(var kind in AppHelper.DependencyKinds)
                {
                    items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, kind, $"{kind}", await context.AssetDependencies.CountAsync(ad => ad.Asset.RepositoryId == id && ad.Asset.Kind == kind)));
                }
            }

            return items;
        }
    }
}
