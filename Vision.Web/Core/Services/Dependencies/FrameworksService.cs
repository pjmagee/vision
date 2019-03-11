namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class FrameworksService
    {
        private readonly VisionDbContext context;

        public FrameworksService(VisionDbContext context) => this.context = context;

        public async Task<IEnumerable<FrameworkDto>> GetAllAsync()
        {
            return await context.Frameworks.Select(framework => new FrameworkDto
            {
                Name = framework.Version,
                FrameworkId = framework.Id,
                Assets = context.AssetFrameworks.Count(af => af.FrameworkId == framework.Id)
            })
            .ToListAsync();
        }

        public async Task<FrameworkDto> GetFrameworkByIdAsync(Guid frameworkId)
        {
            var framework = await context.Frameworks.FindAsync(frameworkId);

            return new FrameworkDto
            {
                Name = framework.Version,
                FrameworkId = framework.Id,
                Assets = await context.AssetFrameworks.CountAsync(assetFramework => assetFramework.FrameworkId == frameworkId)
            };
        }

        public async Task<IEnumerable<RepositoryDto>> GetRepositoriesByFrameworkIdAsync(Guid frameworkId)
        {
            var framework = await context.Frameworks.FindAsync(frameworkId);

            return await context.Repositories
                .Where(repository => context.Assets.Any(asset => asset.RepositoryId == repository.Id && context.AssetFrameworks.Any(assetFramework => assetFramework.FrameworkId == frameworkId && assetFramework.AssetId == asset.Id)))
                .Select(repository => new RepositoryDto
                {
                    Assets = context.Assets.Count(asset => asset.RepositoryId == repository.Id),
                    VersionControlId = repository.VersionControlId,
                    WebUrl = repository.WebUrl,
                    Url = repository.Url,
                    RepositoryId = repository.Id
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssetDto>> GetAssetsByFrameworkIdAsync(Guid frameworkId)
        {
            return await context.Assets
                .Where(asset => context.AssetFrameworks.Any(af => af.AssetId == asset.Id && af.FrameworkId == frameworkId))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Asset = asset.Path,
                    Kind = asset.Kind,
                    Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                    Repository = asset.Repository.WebUrl,
                    RepositoryId = asset.RepositoryId,
                    VersionControlId = asset.Repository.VersionControlId
                })
                .ToListAsync();
        }
    }
}
