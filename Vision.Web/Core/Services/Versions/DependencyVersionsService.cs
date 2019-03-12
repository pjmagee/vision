namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class DependencyVersionsService
    {
        private readonly VisionDbContext context;

        public DependencyVersionsService(VisionDbContext context) => this.context = context;

        public async Task<DependencyVersionDto> GetVersionByIdAsync(Guid dependencyVersionId)
        {
            DependencyVersion version = await context.DependencyVersions.FindAsync(dependencyVersionId);

            return new DependencyVersionDto
            {
                Assets = context.AssetDependencies.Count(assetDependency => assetDependency.DependencyId == version.DependencyId),
                DependencyId = version.DependencyId,
                DependencyVersionId = version.Id,
                IsLatest = version.IsLatest,
                Version = version.Version
            };
        }

        public async Task<IEnumerable<AssetDto>> GetAssetsByDependencyVersionIdAsync(Guid dependencyVersionId)
        {
            return await context.Assets.Where(asset => context.AssetDependencies.Any(ad => ad.AssetId == asset.Id && ad.DependencyVersionId == dependencyVersionId)).Select(asset => new AssetDto
            {                
                AssetId = asset.Id,
                Asset = asset.Path,                          
                Repository = asset.Repository.WebUrl,
                Kind = asset.Kind,
                Dependencies = context.AssetDependencies.Count(a => a.AssetId == asset.Id),
                RepositoryId = asset.RepositoryId,
                VersionControlId = asset.Repository.VersionControlId
            })
            .ToListAsync();
        }
    }
}
