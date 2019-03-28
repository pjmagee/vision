namespace Vision.Web.Core
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class AssetDependencyService : IAssetDependencyService
    {
        private readonly VisionDbContext context;

        public AssetDependencyService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IPaginatedList<AssetDependencyDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.AssetDependencies
                .Where(assetDependency => assetDependency.AssetId == assetId)
                .Select(assetDependency => new AssetDependencyDto
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
                });

            return await PaginatedList<AssetDependencyDto>.CreateAsync(query.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDependencyDto>> GetByDependencyIdAsync(Guid dependencyId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.AssetDependencies
                .Where(ad => ad.DependencyId == dependencyId)
                .Select(assetDependency => new AssetDependencyDto
                {
                    AssetId = assetDependency.AssetId,
                    Version = assetDependency.DependencyVersion.Version,
                    DependencyVersionId = assetDependency.DependencyVersionId,
                    DependencyId = assetDependency.DependencyId,
                    Repository = assetDependency.Asset.Repository.WebUrl,
                    Asset = assetDependency.Asset.Path,
                    IsLatest = assetDependency.DependencyVersion.IsLatest,
                    Dependency = assetDependency.Dependency.Name,
                    RepositoryId = assetDependency.Asset.RepositoryId,
                    VersionControlId = assetDependency.Asset.Repository.VersionControlId
                });

            return await PaginatedList<AssetDependencyDto>.CreateAsync(query.AsNoTracking(), pageIndex, pageSize);
        }
    }
}
