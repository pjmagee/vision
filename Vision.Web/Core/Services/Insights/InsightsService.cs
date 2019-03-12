namespace Vision.Web.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class InsightsService
    {
        private readonly VisionDbContext context;
        private readonly RepositoryAssetsService repositoryAssetsService;

        public InsightsService(VisionDbContext context, RepositoryAssetsService repositoryAssetsService)
        {
            this.context = context;
            this.repositoryAssetsService = repositoryAssetsService;
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
                List<string> assetNames = await repositoryAssetsService.GetAssetPublishNamesByRepositoryIdAsync(repository.Id);

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
