using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Vision.Web.Core
{
    public class AssetService : IAssetService
    {
        private readonly VisionDbContext context;
        private readonly IAggregateAssetExtractor extractor;
        private readonly IDependencyService dependencyService;

        public AssetService(VisionDbContext context, IAggregateAssetExtractor extractor, IDependencyService dependencyService)
        {
            this.context = context;
            this.extractor = extractor;
            this.dependencyService = dependencyService;
        }

        public async Task<AssetDto> GetByIdAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            return new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = context.AssetDependencies.Count(x => x.AssetId == assetId),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId,
                VcsId = asset.Repository.VcsId
            };
        }

        public async Task<IPaginatedList<AssetDto>> GetByDependencyIdAsync(Guid dependencyId, string search, IEnumerable<EcosystemKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.ToIntArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search));
            }

            var paging = query
                .Where(asset => context.AssetDependencies.Any(ad => ad.DependencyId == dependencyId && ad.AssetId == asset.Id))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Asset = asset.Path,
                    Repository = asset.Repository.WebUrl,
                    Kind = asset.Kind,
                    Dependencies = context.AssetDependencies.Count(a => a.AssetId == asset.Id),
                    RepositoryId = asset.RepositoryId,
                    VcsId = asset.Repository.VcsId
                })
                .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByVersionIdAsync(Guid versionId, string search, IEnumerable<EcosystemKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.ToIntArray();

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
                VcsId = asset.Repository.VcsId
            })
            .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByRepositoryIdAsync(Guid repositoryId, string search, IEnumerable<EcosystemKind> kinds, bool dependents, int pageIndex = 1, int pageSize = 10)
        {
            var filter = kinds.ToIntArray();
            var query = context.Assets.AsQueryable();

            if (dependents)
            {
                var dependencies = await dependencyService.GetByRepositoryIdAsync(repositoryId, kinds, search, pageIndex, pageSize);

                if (dependencies.Count == 0)
                {
                    return new PaginatedList<AssetDto>(Enumerable.Empty<AssetDto>().ToList(), 0, 0, 0);
                }

                query = context.Assets.Where(asset => asset.RepositoryId != repositoryId);

                foreach (var dependency in dependencies)
                {
                    query = query.Where(asset => context.AssetDependencies.Any(ad => ad.DependencyId == dependency.DependencyId && ad.AssetId == asset.Id));
                }
            }
            else
            {
                query = context.Assets.Where(asset => asset.RepositoryId == repositoryId).AsQueryable();
            }

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
                Repository = asset.Repository.Url,
                Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                Asset = asset.Path,
                Kind = asset.Kind,
                RepositoryId = asset.RepositoryId,
                VcsId = asset.Repository.VcsId
            })
            .OrderByDescending(asset => asset.Dependencies);

            return PaginatedList<AssetDto>.Create(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetAsync(string search, IEnumerable<EcosystemKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.ToIntArray();

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
                VcsId = asset.Repository.VcsId
            })
            .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByEcosystemIdAsync(Guid EcosystemId, string search, IEnumerable<EcosystemKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.ToIntArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var paging = query
                .Where(asset => context.AssetEcoSystems.Any(ar => ar.AssetId == asset.Id && ar.EcosystemVersion.EcosystemId == EcosystemId))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Asset = asset.Path,
                    Kind = asset.Kind,
                    Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                    Repository = asset.Repository.WebUrl,
                    RepositoryId = asset.RepositoryId,
                    VcsId = asset.Repository.VcsId
                })
                .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByEcosystemVersionIdAsync(Guid EcosystemVersionId, string search, IEnumerable<EcosystemKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Assets.AsQueryable();
            var filter = kinds.ToIntArray();

            if (filter.Any())
            {
                query = query.Where(asset => filter.Contains((int)asset.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(asset => asset.Path.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var paging = query
                .Where(asset => context.AssetEcoSystems.Any(ar => ar.AssetId == asset.Id && ar.EcosystemVersionId == EcosystemVersionId))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Asset = asset.Path,
                    Kind = asset.Kind,
                    Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                    Repository = asset.Repository.WebUrl,
                    RepositoryId = asset.RepositoryId,
                    VcsId = asset.Repository.VcsId
                })
                .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetByVcsIdAsync(Guid VcsId, string search, IEnumerable<EcosystemKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            Vcs versionControl = context.VcsSources.Find(VcsId);

            var query = context.Assets.Where(asset => context.VcsRepositories.Any(repository => repository.VcsId == VcsId && asset.RepositoryId == repository.Id));
            var filter = kinds.ToIntArray();

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
                VcsId = asset.Repository.VcsId
            })
            .OrderByDescending(a => a.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<List<string>> GetPublishedNamesByRepositoryIdAsync(Guid repositoryId)
        {
            List<Asset> assets = await context.Assets.Where(asset => asset.RepositoryId == repositoryId).ToListAsync();
            return assets.Select(asset => extractor.ExtractPublishName(asset)).ToList();
        }

        public async Task<IPaginatedList<AssetDto>> GetByAssetIdAsync(Guid id, string search, IEnumerable<EcosystemKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            var filter = kinds.ToIntArray();
            var query = context.Assets.Where(asset => asset.Id != id).AsQueryable();

            var dependencies = await dependencyService.GetByAssetIdAsync(id, kinds, search, pageIndex, pageSize);

            if (dependencies.Count == 0)
            {
                return new PaginatedList<AssetDto>(Enumerable.Empty<AssetDto>().ToList(), 0, 0, 0);
            }

            foreach (var dependency in dependencies)
            {
                query = query.Where(asset => context.AssetDependencies.Any(ad => ad.DependencyId == dependency.DependencyId && ad.AssetId == asset.Id));
            }

            var paging = query.Select(asset => new AssetDto
            {
                Asset = asset.Path,
                AssetId = asset.Id,
                Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id),
                Repository = asset.Repository.Url,
                RepositoryId = asset.RepositoryId,
                VcsId = asset.Repository.VcsId
            })
            .OrderByDescending(a => a.Dependencies);

            return PaginatedList<AssetDto>.Create(paging, pageIndex, pageSize);
        }
    }
}
