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
        private readonly IAggregateAssetExtractor extractor;

        public DependencyService(VisionDbContext context, IAggregateAssetExtractor extractor)
        {
            this.context = context;
            this.extractor = extractor;
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

        public async Task<IPaginatedList<DependencyDto>> GetAsync(IEnumerable<DependencyKind> kinds, string search, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Dependencies.AsQueryable();
            var filter = kinds.ToIntArray();

            if (filter.Any())
            {
                query = query.Where(dependency => filter.Contains((int)dependency.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(dependency => dependency.Name.Contains(search) || context.DependencyVersions.Any(dependencyVersion => dependencyVersion.DependencyId == dependency.Id && dependencyVersion.Version.Contains(search)));
            }

            var paging = query.Select(dependency => new DependencyDto
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

        public async Task<IPaginatedList<DependencyDto>> GetByRepositoryIdAsync(Guid repositoryId, IEnumerable<DependencyKind> kinds, string search, int pageIndex = 1, int pageSize = 10)
        {
            var filter = kinds.ToIntArray();
            Repository repository = await context.Repositories.FindAsync(repositoryId);
            List<string> assetNames = await context.Assets.Where(asset => asset.RepositoryId == repositoryId).Select(asset => extractor.ExtractPublishName(asset)).ToListAsync();

            var query = context.Dependencies.AsQueryable();                

            if (filter.Any())
            {
                query = query.Where(dependency => filter.Contains((int)dependency.Kind));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(dependency => dependency.Name.Contains(search));
            }

            query = query.Where(dependency => assetNames.Contains(dependency.Name) || context.DependencyVersions.Any(dv => dv.DependencyId == dependency.Id && string.Equals(dv.ProjectUrl, repository.Url) || string.Equals(dv.ProjectUrl, repository.WebUrl)));

            var paging = query.Select(dependency => new DependencyDto
            {
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                DependencyId = dependency.Id,
                Kind = dependency.Kind,
                Name = dependency.Name,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id)
            })
            .OrderByDescending(d => d.Assets)
            .ThenByDescending(d => d.Versions);

            return await PaginatedList<DependencyDto>.CreateAsync(paging, pageIndex, pageSize);
        }
    }
}
