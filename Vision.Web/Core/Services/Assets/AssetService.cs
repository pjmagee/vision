namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class AssetService : IAssetService
    {
        private readonly VisionDbContext context;
        private readonly IAggregateAssetExtractor extractor;

        public AssetService(VisionDbContext context, IAggregateAssetExtractor extractor)
        {
            this.context = context;
            this.extractor = extractor;
        }

        public async Task<AssetDto> GetByIdAsync(Guid assetId)
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

        public async Task<IPaginatedList<AssetDto>> GetByDependencyIdAsync(Guid dependencyId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.Cast<int>().ToArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search));
            }

            var paging = query.Where(asset => context.AssetDependencies.Any(ad => ad.DependencyId == dependencyId && ad.AssetId == asset.Id))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Asset = asset.Path,
                    Repository = asset.Repository.WebUrl,
                    Kind = asset.Kind,
                    Dependencies = context.AssetDependencies.Count(a => a.AssetId == asset.Id),
                    RepositoryId = asset.RepositoryId,
                    VersionControlId = asset.Repository.VersionControlId
                })
                .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByVersionIdAsync(Guid versionId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.Cast<int>().ToArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search));
            }

            query = query.Where(asset => context.AssetDependencies.Any(ad => ad.DependencyVersionId == versionId && ad.AssetId == asset.Id));            
                
            var paging = query.Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Asset = asset.Path,
                Repository = asset.Repository.WebUrl,
                Kind = asset.Kind,
                Dependencies = context.AssetDependencies.Count(a => a.AssetId == asset.Id),
                RepositoryId = asset.RepositoryId,
                VersionControlId = asset.Repository.VersionControlId
            })
            .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByRepositoryIdAsync(Guid repositoryId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null)
        {
            var query = context.Assets.Where(asset => asset.RepositoryId == repositoryId).AsQueryable();
            var filter = kinds.Cast<int>().ToArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if(!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search));
            }

            var paging = query.Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Repository = asset.Repository.Url,
                Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                Asset = asset.Path,
                Kind = asset.Kind,
                RepositoryId = asset.RepositoryId,
                VersionControlId = asset.Repository.VersionControlId
            })
            .OrderByDescending(asset => asset.Dependencies);    

            return await PaginatedList<AssetDto>.CreateAsync(paging.AsNoTracking(), pageIndex, pageSize);
        }
        
        public async Task<IPaginatedList<AssetDto>> GetAsync(IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.Cast<int>().ToArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search));
            }

            var paging = query.Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Asset = asset.Path,
                Kind = asset.Kind,
                Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                Repository = asset.Repository.WebUrl,
                RepositoryId = asset.RepositoryId,
                VersionControlId = asset.Repository.VersionControlId
            })
            .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByFrameworkIdAsync(Guid frameworkId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.Cast<int>().ToArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var paging = query
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
                .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByVersionControlIdAsync(Guid versionControlId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

            var query = context.Assets.Where(asset => context.Repositories.Any(repository => repository.VersionControlId == versionControlId && asset.RepositoryId == repository.Id));
            var filter = kinds.Cast<int>().ToArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search));
            }

            var paging = query.Select(asset => new AssetDto
            {
                Asset = asset.Path,
                AssetId = asset.Id,
                Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id),
                Repository = asset.Repository.Url,
                RepositoryId = asset.RepositoryId,
                VersionControlId = versionControlId
            })
            .OrderByDescending(a => a.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<List<string>> GetPublishedNamesByRepositoryIdAsync(Guid repositoryId)
        {
            var assets = await context.Assets.Where(asset => asset.RepositoryId == repositoryId).AsNoTracking().ToListAsync();
            return assets.Select(asset => extractor.ExtractPublishName(asset)).ToList();
        }
    }
}
