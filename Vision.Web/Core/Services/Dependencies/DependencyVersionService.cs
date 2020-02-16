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
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == version.DependencyId),
                DependencyId = version.DependencyId,
                DependencyVersionId = version.Id,
                IsLatest = version.IsLatest,
                Version = version.Version,
                Vulnerabilities = context.Vulnerabilities.Count(r => r.DependencyVersionId == dependencyVersionId)
            };
        }

        public async Task<IPaginatedList<DependencyVersionDto>> GetByDependencyIdAsync(Guid dependencyId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.DependencyVersions
                .Where(version => version.DependencyId == dependencyId)
                .Select(version => new DependencyVersionDto
                {
                    Assets = context.AssetDependencies.Count(ad => ad.DependencyVersionId == version.Id),
                    IsLatest = version.IsLatest,
                    DependencyId = version.DependencyId,
                    DependencyVersionId = version.Id,
                    Version = version.Version,
                    Vulnerabilities = context.Vulnerabilities.Count(v => v.DependencyVersionId == version.Id)
                })
                .OrderBy(v => v.Assets)
                .ThenBy(v => v.IsLatest);

            return await PaginatedList<DependencyVersionDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
