﻿namespace Vision.Core
{
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
                    Kind = assetDependency.Asset.Kind,
                    DependencyId = assetDependency.DependencyId,
                    DependencyVersionId = assetDependency.DependencyVersionId,
                    IsLatest = assetDependency.DependencyVersion.IsLatest,
                    RepositoryId = assetDependency.Asset.RepositoryId,
                    VcsId = assetDependency.Asset.Repository.VcsId
                });

            return await PaginatedList<AssetDependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDependencyDto>> GetByDependencyIdAsync(Guid depId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.AssetDependencies
                .Where(ad => ad.DependencyId == depId)
                .Select(assetDependency => new AssetDependencyDto
                {
                    AssetId = assetDependency.AssetId,
                    Version = assetDependency.DependencyVersion.Version,
                    DependencyVersionId = assetDependency.DependencyVersionId,
                    DependencyId = assetDependency.DependencyId,
                    Repository = assetDependency.Asset.Repository.WebUrl,
                    Asset = assetDependency.Asset.Path,
                    Kind = assetDependency.Asset.Kind,
                    IsLatest = assetDependency.DependencyVersion.IsLatest,
                    Dependency = assetDependency.Dependency.Name,
                    RepositoryId = assetDependency.Asset.RepositoryId,
                    VcsId = assetDependency.Asset.Repository.VcsId
                });

            return await PaginatedList<AssetDependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
