namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    
    public class RepositoriesService
    {
        private readonly VisionDbContext context;
        private readonly ICICDBuildsService buildsService;
        private readonly RepositoryAssetsService repositoryAssetsService;

        public RepositoriesService(VisionDbContext context, ICICDBuildsService buildsService, RepositoryAssetsService repositoryAssetsService)
        {
            this.context = context;
            this.buildsService = buildsService;
            this.repositoryAssetsService = repositoryAssetsService;
        }

        public async Task<RepositoryDto> GetRepositoryByIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            return new RepositoryDto
            {                
                VersionControlId = repository.VersionControlId,
                Assets = await context.Assets.CountAsync(asset => asset.RepositoryId == repositoryId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id
            };
        }

        public async Task<PaginatedList<AssetDto>> GetAssetsByRepositoryId(Guid repositoryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Assets.Where(asset => asset.RepositoryId == repositoryId).Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Repository = asset.Repository.Url,
                Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                Asset = asset.Path,
                Kind = asset.Kind,
                RepositoryId = asset.RepositoryId,
                VersionControlId = asset.Repository.VersionControlId
            });

            return await PaginatedList<AssetDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IEnumerable<DependencyDto>> GetDependenciesByRepositoryId(Guid repositoryId)
        {
            // Find all Dependencies who's Project URl (META DATA FROM DEPENDENCY REGISTRY) matches this Repositories Git or Web Url
            // TODO: Ensure all XpertHR Assets which create dependencies (nuspec files) have the repository in the nuspec <RepositoryUrl></RepositoryUrl>
            Repository repository = await context.Repositories.FindAsync(repositoryId);
            List<string> assetNames = await repositoryAssetsService.GetAssetPublishNamesByRepositoryIdAsync(repositoryId);

            return await context.Dependencies.Where(dependency => assetNames.Contains(dependency.Name) || string.Equals(dependency.RepositoryUrl, repository.Url) || string.Equals(dependency.RepositoryUrl, repository.WebUrl)).Select(dependency => new DependencyDto
            {
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                DependencyId = dependency.Id,
                Kind = dependency.Kind,
                Name = dependency.Name,
                RepositoryUrl = dependency.RepositoryUrl,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id)
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<AssetDto>> GetDependentsByRepositoryId(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            return await context.Assets
                .Where(asset => context.AssetDependencies.Where(ad => ad.AssetId == asset.Id).Any(ad => context.Dependencies.Any(d => ad.DependencyId == d.Id && string.Equals(d.RepositoryUrl, repository.Url) || string.Equals(d.RepositoryUrl, repository.WebUrl))))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Repository = asset.Repository.Url,
                    Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                    Asset = asset.Path,
                    RepositoryId = asset.RepositoryId,
                    Kind = asset.Kind,
                    VersionControlId = asset.Repository.VersionControlId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FrameworkDto>> GetFrameworksByRepositoryId(Guid repositoryId)
        {
            return await context.Frameworks
                .Where(fw => context.AssetFrameworks.Any(af => af.FrameworkId == fw.Id && context.Assets.Any(a => a.RepositoryId == repositoryId && af.AssetId == a.Id)))
                .Select(framework => new FrameworkDto
                {
                    Assets = context.AssetFrameworks.Count(assetFramework => assetFramework.FrameworkId == framework.Id && assetFramework.Asset.RepositoryId == repositoryId),
                    FrameworkId = framework.Id,
                    Name = framework.Version
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CiCdBuildDto>> GetCiCdsByRepositoryId(Guid repositoryId)
        {
            return await buildsService.GetBuildsByRepositoryIdAsync(repositoryId);
        }
    }
}