namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class AssetsService
    {
        private readonly VisionDbContext context;

        public AssetsService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<AssetDto> GetAssetByIdAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            return new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = await context.AssetDependencies.CountAsync(x => x.AssetId == assetId),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId,
                VersionControlId = asset.Repository.VersionControlId
            };
        }

        public async Task<IEnumerable<AssetDependencyDto>> GetDependenciesByAssetIdAsync(Guid assetId)
        {
            return await context.AssetDependencies.Where(assetDependency => assetDependency.AssetId == assetId).Select(assetDependency => new AssetDependencyDto
            {
                Repository = assetDependency.Asset.Repository.WebUrl,
                Dependency = assetDependency.Dependency.Name,
                Version = assetDependency.DependencyVersion.Version,
                AssetId = assetDependency.AssetId,
                Asset = assetDependency.Asset.Path,
                DependencyId = assetDependency.DependencyId,
                DependencyVersionId = assetDependency.DependencyVersionId,
                IsLatest = assetDependency.DependencyVersion.IsLatest,
                RepositoryId = assetDependency.Asset.RepositoryId,
                VersionControlId = assetDependency.Asset.Repository.VersionControlId
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<FrameworkDto>> GetFrameworksByAssetId(Guid assetId)
        {
            return await context.Frameworks
                .Where(x => context.AssetFrameworks.Any(af => af.AssetId == assetId))
                .Select(x => new FrameworkDto { FrameworkId = x.Id, Name = x.Version })
                .ToListAsync();
        }        
    }
}
