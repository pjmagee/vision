using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Vision.Core
{
    public class MetricService : IMetricService
    {
        private readonly VisionDbContext context;
        private readonly IMemoryCache cache;

        public MetricService(VisionDbContext context, IMemoryCache cache)
        {
            this.context = context;
            this.cache = cache;
        }

        const string Counts = "Dashboard.Counts";

        public async Task<IEnumerable<MetricItem>> GetCountsAsync()
        {
            if (!cache.TryGetValue(Counts, out List<MetricItem> metrics))
            {
                IEnumerable<MetricItem> dependencyMetricCounts = Constants.EcosystemKinds.Select(kind => new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, kind, $"{kind} dependencies", context.Dependencies.Count(x => x.Kind == kind)));
                IEnumerable<MetricItem> assetMetricCounts = Constants.EcosystemKinds.Select(kind => new MetricItem(MetricAlertKind.Standard, CategoryKind.Asset, kind, $"{kind} assets", context.Assets.Count(x => x.Kind == kind)));

                var otherMetricCounts = new MetricItem[]
                {
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.VersionControl, $"Version controls", await context.VcsSources.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Repository, $"Repositories", await context.VcsRepositories.CountAsync()),

                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Registry, $"Registries", await context.EcoRegistrySources.CountAsync()),

                    new MetricItem(MetricAlertKind.Standard, CategoryKind.CiCd, $"CiCd", await context.CicdSources.CountAsync()),

                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Asset, $"Assets", await context.Assets.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Asset dependencies", await context.AssetDependencies.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Asset, $"Asset ecosystems", await context.AssetEcoSystems.CountAsync()),

                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Dependencies", await context.Dependencies.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Dependency versions", await context.DependencyVersions.CountAsync()),
                    new MetricItem(MetricAlertKind.Warning, CategoryKind.Dependency, $"Vulnerabilities", await context.Vulnerabilities.CountAsync()),

                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Ecosystem, $"Ecosystems", await context.Ecosystems.CountAsync()),
                    new MetricItem(MetricAlertKind.Standard, CategoryKind.Ecosystem, $"Ecosystem versions", await context.EcosystemVersions.CountAsync()),
                };

                metrics = otherMetricCounts.Concat(dependencyMetricCounts.Concat(assetMetricCounts)).ToList();

                cache.Set(Counts, metrics, DateTimeOffset.Now.AddMinutes(5));
            }

            return metrics;
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
            DependencyVersion dependencyVersion = await context.DependencyVersions.FindAsync(dependencyVersionId);

            List<MetricItem> items = new List<MetricItem>
            {
                new MetricItem(MetricAlertKind.Standard, CategoryKind.DependencyVersion, dependencyVersion.Dependency.Kind, $"Dependency", dependencyVersion.Dependency.Name),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.DependencyVersion, $"Version", dependencyVersion.Version),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.DependencyVersion, $"Latest", dependencyVersion.IsLatest.ToYesNo()),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.DependencyVersion, $"Assets", await context.AssetDependencies.CountAsync(ad => ad.DependencyVersionId == dependencyVersionId)),
            };

            return items;
        }

        private async Task<IEnumerable<MetricItem>> GetRepositoryMetricsAsync(Guid repositoryId)
        {
            VcsRepository repository = await context.VcsRepositories.FindAsync(repositoryId);

            List<MetricItem> items = new List<MetricItem>
            {
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Repository, $"Assets", context.Assets.Count(ad => ad.RepositoryId == repositoryId)),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Repository, $"Kind", repository.Vcs.Kind),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Repository, $"Ignored", repository.Url.ToYesNo())
            };

            foreach (var kind in Constants.EcosystemKinds)
            {
                items.Add(new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, kind, $"{kind}", context.AssetDependencies.Count(ad => ad.Asset.RepositoryId == repositoryId && ad.Asset.Kind == kind)));
            }

            return items;
        }

        private async Task<IEnumerable<MetricItem>> GetDependencyMetricsAsync(Guid dependencyId)
        {
            List<MetricItem> items = new List<MetricItem>
            {
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Ecosystem, $"Assets", await context.AssetDependencies.CountAsync(ad => ad.DependencyId == dependencyId)),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Versions", await context.DependencyVersions.CountAsync(ad => ad.DependencyId == dependencyId))
            };

            IQueryable<IGrouping<Guid, AssetDependency>> dependencies = context.AssetDependencies.Where(ad => ad.DependencyId == dependencyId).GroupBy(dv => dv.DependencyVersionId);

            Guid mostId = dependencies.OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            Guid leastId = dependencies.OrderBy(g => g.Count()).Select(g => g.Key).FirstOrDefault();

            DependencyVersion most = await context.DependencyVersions.FindAsync(mostId);
            DependencyVersion least = await context.DependencyVersions.FindAsync(leastId);
            DependencyVersion latest = await context.DependencyVersions.FirstOrDefaultAsync(dv => dv.DependencyId == dependencyId && dv.IsLatest);

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
            Asset asset = await context.Assets.FindAsync(assetId);

            List<MetricItem> items = new List<MetricItem>
            {
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Dependency, $"Dependencies", await context.AssetDependencies.CountAsync(ad => ad.AssetId == assetId)),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Ecosystem, $"Runtimes", await context.AssetEcoSystems.CountAsync(ad => ad.AssetId == assetId)),
                new MetricItem(MetricAlertKind.Standard, CategoryKind.Asset, asset.Kind, $"Kind", asset.Kind)
            };

            return items;
        }
    }
}
