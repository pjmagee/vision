namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class DependencyVersionService : IDependencyVersionService
    {
        private readonly VisionDbContext context;

        public DependencyVersionService(VisionDbContext context) => this.context = context;

        public async Task<DependencyVersionDto> GetByIdAsync(Guid dependencyVersionId)
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

        public async Task<IPaginatedList<DependencyVersionDto>> GetByDependencyIdAsync(Guid dependencyId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.DependencyVersions
                .Where(dependencyVersion => dependencyVersion.DependencyId == dependencyId)
                .Select(dependencyVersion => new DependencyVersionDto
                {
                    Assets = context.AssetDependencies.Count(assetDependency => assetDependency.DependencyVersionId == dependencyVersion.Id),
                    IsLatest = dependencyVersion.IsLatest,
                    DependencyId = dependencyVersion.DependencyId,
                    DependencyVersionId = dependencyVersion.Id,
                    Version = dependencyVersion.Version
                })
                .OrderBy(v => v.IsLatest);

            return await PaginatedList<DependencyVersionDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
