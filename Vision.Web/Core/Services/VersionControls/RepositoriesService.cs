namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class RepositoriesService
    {
        private readonly VisionDbContext context;
        private readonly ICICDBuildsService buildsService;

        public RepositoriesService(VisionDbContext context, ICICDBuildsService buildsService)
        {
            this.context = context;
            this.buildsService = buildsService;
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

        public async Task<IEnumerable<AssetDto>> GetAssetsByRepositoryId(Guid repositoryId)
        {
            return await context.Assets.Where(asset => asset.RepositoryId == repositoryId).Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Repository = asset.Repository.Url,
                Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                Asset = asset.Path,
                Kind = asset.Kind,
                RepositoryId = asset.RepositoryId,
                VersionControlId = asset.Repository.VersionControlId
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<DependencyDto>> GetDependenciesByRepositoryId(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            return await context.Dependencies.Where(dependency => string.Equals(dependency.RepositoryUrl, repository.Url) || string.Equals(dependency.RepositoryUrl, repository.WebUrl)).Select(dependency => new DependencyDto
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
            return await context.Assets
                .Where(asset => asset.RepositoryId == repositoryId) // assets in this repository
                .SelectMany(asset => context.AssetFrameworks.Where(x => x.AssetId == asset.Id)) // all asset frameworks that this asset uses
                .Select(assetFramework => assetFramework.Framework).Distinct() // select the linked unique frameworks and distinct them
                .Select(framework => new FrameworkDto
                {
                    Assets = context.AssetFrameworks.Count(assetFramework => assetFramework.FrameworkId == framework.Id),
                    FrameworkId = framework.Id,
                    Name = framework.Version,
                    
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CiCdBuildDto>> GetCiCdsByRepositoryId(Guid repositoryId)
        {
            return await buildsService.GetBuildsByRepositoryIdAsync(repositoryId);
        }
    }
}