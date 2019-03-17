namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class MetricItemsService
    {
        private readonly VisionDbContext context;
        private readonly RepositoryAssetsService repositoryAssetsService;

        public MetricItemsService(VisionDbContext context, RepositoryAssetsService repositoryAssetsService)
        {
            this.context = context;
            this.repositoryAssetsService = repositoryAssetsService;
        }

        public async Task<IEnumerable<MetricItem>> GetCountsAsync()
        {
            IEnumerable<Task<MetricItem>> dependencies =
                AppHelper.DependencyKinds.Select(async kind => new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, kind, $"{kind} dependencies", await context.Dependencies.CountAsync(x => x.Kind == kind)));

            IEnumerable<Task<MetricItem>> assets =
                AppHelper.DependencyKinds.Select(async kind => new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Assets, kind, $"{kind} assets", await context.Assets.CountAsync(x => x.Kind == kind)));

            var otherCounts = new MetricItem[]
            {
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.VersionControls, $"Version control systems", await context.VersionControls.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Registries, $"Registry sources", await context.Registries.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.CiCds, $"CI/CD sources", await context.CiCds.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Repositories, $"Repositories", await context.Repositories.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Assets, $"Assets", await context.Assets.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, $"Dependencies", await context.Dependencies.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Frameworks, $"Frameworks", await context.Frameworks.CountAsync()),
            };

            return otherCounts.Concat(await Task.WhenAll(dependencies.Concat(assets).ToArray()));
        }

        public async Task<IEnumerable<MetricItems<RepositoryDto>>> GetRepositoriesMetricsByVersionControlIdAsync(Guid versionControlId)
        {
            IQueryable<Repository> orderedByLargest = context.Repositories.Where(repository => repository.VersionControlId == versionControlId).OrderBy(repository => context.Assets.Count(asset => asset.RepositoryId == repository.Id));

            return new MetricItems<RepositoryDto>[]
            {
                new MetricItems<RepositoryDto>(MetricAlertKind.Standard, MetricCategoryKind.Repositories, "Top 5 smallest repositories", await orderedByLargest.TakeLast(5).Select(repository => new RepositoryDto {  }).ToArrayAsync()),
                new MetricItems<RepositoryDto>(MetricAlertKind.Standard, MetricCategoryKind.Repositories, "Top 5 largest repositories", await orderedByLargest.Take(5).Select(repository => new RepositoryDto {  }).ToArrayAsync())
            };
        }

        public async Task<IEnumerable<MetricItems<AssetDto>>> GetAssetInsightsByKindAsync(DependencyKind dependencyKind)
        {
            AssetDto[] assets = await context.Assets.Where(asset => asset.Kind == dependencyKind)
                .OrderByDescending(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Repository = asset.Repository.Url,
                    Asset = asset.Path,
                    Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id),
                    VersionControlId = asset.Repository.VersionControlId,
                    Kind = asset.Kind,
                    RepositoryId = asset.RepositoryId
                })
                .ToArrayAsync();

            return new[]
            {
                new MetricItems<AssetDto>(MetricAlertKind.Standard, MetricCategoryKind.Assets, $"{dependencyKind} assets", assets)
            };
        }

        public async Task<IEnumerable<MetricItems<RepositoryDto>>> GetPublishingRepositoriesByDependencyKind(DependencyKind dependencyKind)
        {
            List<RepositoryDto> repositories = new List<RepositoryDto>();

            foreach (Repository repository in context.Repositories)
            {
                List<string> assetNames = await repositoryAssetsService.GetPublishedNamesByRepositoryIdAsync(repository.Id);

                bool publishes = await context.Dependencies
                    .Where(d => d.Kind == dependencyKind)
                    .AnyAsync(dependency => assetNames.Contains(dependency.Name) || (string.Equals(dependency.RepositoryUrl, repository.Url) || string.Equals(dependency.RepositoryUrl, repository.WebUrl)));

                if(publishes)
                {
                    repositories.Add(new RepositoryDto
                    {
                        Assets = context.Assets.Count(a => a.RepositoryId == repository.Id),
                        RepositoryId = repository.Id,
                        Url = repository.Url,
                        WebUrl = repository.WebUrl,
                        VersionControlId = repository.VersionControlId,
                    });
                }
            }

            return new[]
            {
                new MetricItems<RepositoryDto>(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, $"Repositories that publish {dependencyKind} dependencies", repositories)
            };
        }

        public async Task<IEnumerable<MetricItems<DependencyDto>>> GetDependencyInsightsByKindAsync(DependencyKind dependencyKind)
        {
            DependencyDto[] dependencies = await context.Dependencies.Where(dependency => dependency.Kind == dependencyKind)
                .OrderByDescending(dependency => context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id))
                .Select(dependency => new DependencyDto
                {
                    DependencyId = dependency.Id,
                    Name = dependency.Name,
                    Kind = dependency.Kind,
                    Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                    Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id),
                    RepositoryUrl = dependency.RepositoryUrl                    
                })
                .ToArrayAsync();

            return new[]
            {
                new MetricItems<DependencyDto>(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, $"{dependencyKind} dependencies", dependencies)
            };
        }
    }
}
