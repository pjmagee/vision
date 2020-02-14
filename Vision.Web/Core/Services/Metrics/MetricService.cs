﻿namespace Vision.Web.Core
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
                IEnumerable<MetricItem> dependencies = AppHelper.DependencyKinds.Select(kind => new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, kind, $"{kind} dependencies", context.Dependencies.Count(x => x.Kind == kind)));
                IEnumerable<MetricItem> assets = AppHelper.DependencyKinds.Select(kind => new MetricItem(MetricAlertKind.Standard, CategoryKind.Asset, kind, $"{kind} assets", context.Assets.Count(x => x.Kind == kind)));

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

                counts = otherCounts.Concat(dependencies.Concat(assets)).ToList();

                cache.Set(Counts, counts, DateTimeOffset.Now.AddMinutes(5));
            }

            return counts;
        }

        public async Task<IEnumerable<MetricItem>> GetMetricsAsync(Guid id, CategoryKind targetKind)
        {
            return targetKind switch
            {
                CategoryKind.Asset => await GetAssetMetricsAsync(id),
                CategoryKind.Dependency => await GetDependencyMetricsAsync(id),
                CategoryKind.Repository => await GetRepositoryMetricsAsync(id),
                CategoryKind.DependencyVersion => await GetDependencyVersionMetricsAsync(id),
                _ => Enumerable.Empty<MetricItem>()
            };
        }

        private async Task<IEnumerable<MetricItem>> GetDependencyVersionMetricsAsync(Guid dependencyVersionId)
        {
            List<MetricItem> items = new List<MetricItem>();
            DependencyVersion dependencyVersion = await context.DependencyVersions.FindAsync(dependencyVersionId);

            items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Assets", await context.AssetDependencies.CountAsync(ad => ad.DependencyVersionId == dependencyVersionId)));
            items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Latest", dependencyVersion.IsLatest.ToYesNo()));

            return items;
        }

        private async Task<IEnumerable<MetricItem>> GetRepositoryMetricsAsync(Guid repositoryId)
        {
            List<MetricItem> items = new List<MetricItem>();

            items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Assets", await context.Assets.CountAsync(ad => ad.RepositoryId == repositoryId)));

            foreach (var kind in AppHelper.DependencyKinds)
            {
                items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, kind, $"{kind}", await context.AssetDependencies.CountAsync(ad => ad.Asset.RepositoryId == repositoryId && ad.Asset.Kind == kind)));
            }

            return items;
        }

        private async Task<IEnumerable<MetricItem>> GetDependencyMetricsAsync(Guid dependencyId)
        {
            List<MetricItem> items = new List<MetricItem>();

            items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Assets", await context.AssetDependencies.CountAsync(ad => ad.DependencyId == dependencyId)));
            items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Versions", await context.DependencyVersions.CountAsync(ad => ad.DependencyId == dependencyId)));

            var dependencies = context.AssetDependencies.Where(ad => ad.DependencyId == dependencyId).GroupBy(dv => dv.DependencyVersionId);

            Guid mostId = await dependencies.OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefaultAsync();
            Guid leastId = await dependencies.OrderBy(g => g.Count()).Select(g => g.Key).FirstOrDefaultAsync();

            var most = await context.DependencyVersions.FindAsync(mostId);
            var least = await context.DependencyVersions.FindAsync(leastId);

            var latest = await context.DependencyVersions.FirstOrDefaultAsync(dv => dv.DependencyId == dependencyId && dv.IsLatest);

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

            return items;
        }

        private async Task<IEnumerable<MetricItem>> GetAssetMetricsAsync(Guid assetId)
        {
            List<MetricItem> items = new List<MetricItem>();

            items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Dependencies", await context.AssetDependencies.CountAsync(ad => ad.AssetId == assetId)));
            items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Framework, $"Frameworks", await context.AssetFrameworks.CountAsync(ad => ad.AssetId == assetId)));

            return items;
        }
    }
}
