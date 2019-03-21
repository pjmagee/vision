namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class DependencyService : IDependencyService
    {
        private readonly VisionDbContext context;
        private readonly IAssetService assetService;

        public DependencyService(VisionDbContext context, IAssetService assetService)
        {
            this.context = context;
            this.assetService = assetService;
        }

        public async Task<IPaginatedList<DependencyDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var paging = context.Dependencies.Select(dependency => new DependencyDto
            {
                DependencyId = dependency.Id,
                Name = dependency.Name,
                Kind = dependency.Kind,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id),
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
            })
            .OrderByDescending(d => d.Assets)
            .ThenByDescending(d => d.Versions);

            return await PaginatedList<DependencyDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<AssetDto>> GetDependentsByRepositoryId(Guid repositoryId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            var dependencies = await GetByRepositoryIdAsync(repositoryId, pageSize: 1000);

            if (dependencies.Count == 0)
                return new PaginatedList<AssetDto>(Enumerable.Empty<AssetDto>().ToList(), 0, 0, 0);

            var assets = context.Assets.AsQueryable();
            var filter = kinds.Cast<int>().ToArray();

            if (filter.Any())
            {
                assets = assets.Where(asset => filter.Contains((int)asset.Kind));
            }

            foreach (var dependency in dependencies)
            {
                assets = assets.Where(asset => context.AssetDependencies.Any(ad => ad.DependencyId == dependency.DependencyId && ad.AssetId == asset.Id));
            }

            var paging = assets.Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Repository = asset.Repository.Url,
                Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId,
                Kind = asset.Kind,
                VersionControlId = asset.Repository.VersionControlId
            })
            .OrderByDescending(asset => asset.Dependencies);

            return await PaginatedList<AssetDto>.CreateAsync(paging.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<IPaginatedList<DependencyDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10)
        {
            // Find all Dependencies who's Project URl (META DATA FROM DEPENDENCY REGISTRY) matches this Repositories Git or Web Url
            // TODO: Ensure all XpertHR Assets which create dependencies (nuspec files) have the repository in the nuspec <RepositoryUrl></RepositoryUrl>

            Repository repository = await context.Repositories.FindAsync(repositoryId);
            List<string> assetNames = await assetService.GetPublishedNamesByRepositoryIdAsync(repositoryId);

            var query = context.Dependencies
                .Where(dependency => assetNames.Contains(dependency.Name) || context.DependencyVersions.Any(dv => dv.DependencyId == dependency.Id && string.Equals(dv.ProjectUrl, repository.Url) || string.Equals(dv.ProjectUrl, repository.WebUrl)))
                .Select(dependency => new DependencyDto
                {
                    Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                    DependencyId = dependency.Id,
                    Kind = dependency.Kind,
                    Name = dependency.Name,
                    Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id)
                })
                .OrderByDescending(d => d.Assets)
                .ThenByDescending(d => d.Versions);

            return await PaginatedList<DependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<DependencyDto>> GetByRegistryIdAsync(Guid registryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Dependencies
                 .Where(d => d.RegistryId == registryId)
                 .Select(entity => new DependencyDto
                 {
                     Name = entity.Name,
                     Versions = context.DependencyVersions.Count(x => x.DependencyId == entity.Id),
                     Assets = context.AssetDependencies.Count(x => x.DependencyId == entity.Id),
                     DependencyId = entity.Id,
                     Kind = entity.Kind
                 })
                 .OrderByDescending(d => d.Assets)
                 .ThenByDescending(d => d.Versions);

            return await PaginatedList<DependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<DependencyDto>> GetByKindsAsync(DependencyKind[] kinds, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Dependencies.Where(dependency => kinds.Contains(dependency.Kind)).Select(dependency => new DependencyDto
            {
                DependencyId = dependency.Id,
                Name = dependency.Name,
                Kind = dependency.Kind,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id),
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id)
            })
            .OrderByDescending(d => d.Assets)
            .ThenByDescending(d => d.Versions);

            return await PaginatedList<DependencyDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<DependencyDto> GetByIdAsync(Guid dependencyId)
        {
            Dependency dependency = await context.Dependencies.FindAsync(dependencyId);

            return new DependencyDto
            {
                Name = dependency.Name,
                Kind = dependency.Kind,
                DependencyId = dependency.Id,
                Assets = await context.AssetDependencies.CountAsync(assetDependency => assetDependency.DependencyId == dependencyId),
                Versions = await context.DependencyVersions.CountAsync(dependencyVersion => dependencyVersion.DependencyId == dependencyId),
            };
        }
    }
}
