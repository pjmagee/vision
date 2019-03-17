namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class FrameworksService
    {
        private readonly VisionDbContext context;

        public FrameworksService(VisionDbContext context) => this.context = context;

        public async Task<PaginatedList<FrameworkDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Frameworks.Select(framework => new FrameworkDto
            {
                Name = framework.Version,
                FrameworkId = framework.Id,
                Assets = context.AssetFrameworks.Count(af => af.FrameworkId == framework.Id)
            })
            .OrderByDescending(f => f.Assets);

            return await PaginatedList<FrameworkDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<FrameworkDto> GetByIdAsync(Guid frameworkId)
        {
            var framework = await context.Frameworks.FindAsync(frameworkId);

            return new FrameworkDto
            {
                Name = framework.Version,
                FrameworkId = framework.Id,
                Assets = await context.AssetFrameworks.CountAsync(assetFramework => assetFramework.FrameworkId == frameworkId)
            };
        }

        public async Task<PaginatedList<FrameworkDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Frameworks
                .Where(fw => context.AssetFrameworks.Any(af => af.AssetId == assetId && af.FrameworkId == fw.Id))
                .Select(fw => new FrameworkDto
                {
                    FrameworkId = fw.Id,
                    Name = fw.Version,
                    Assets = context.AssetFrameworks.Count(af => af.FrameworkId == fw.Id)
                })
                .OrderByDescending(fw => fw.Assets);

            return await PaginatedList<FrameworkDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<PaginatedList<FrameworkDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Frameworks
                .Where(fw => context.AssetFrameworks.Any(af => af.FrameworkId == fw.Id && context.Assets.Any(a => a.RepositoryId == repositoryId && af.AssetId == a.Id)))
                .Select(framework => new FrameworkDto
                {
                    Assets = context.AssetFrameworks.Count(assetFramework => assetFramework.FrameworkId == framework.Id && assetFramework.Asset.RepositoryId == repositoryId),
                    FrameworkId = framework.Id,
                    Name = framework.Version
                })
                .OrderByDescending(f => f.Assets);

            return await PaginatedList<FrameworkDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
