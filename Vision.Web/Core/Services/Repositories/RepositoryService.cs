namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class RepositoryService : IRepositoryService
    {
        private readonly VisionDbContext context;
        private readonly ICICDProvider buildsService;

        public RepositoryService(VisionDbContext context, ICICDProvider buildsService)
        {
            this.context = context;
            this.buildsService = buildsService;
        }

        public async Task<RepositoryDto> ToggleIgnoreAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);
            repository.IsIgnored = !repository.IsIgnored;
            await context.SaveChangesAsync();

            return new RepositoryDto
            {
                VersionControlId = repository.VersionControlId,
                Assets = await context.Assets.CountAsync(asset => asset.RepositoryId == repositoryId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            };
        }

        public async Task<RepositoryDto> GetByIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            return new RepositoryDto
            {
                VersionControlId = repository.VersionControlId,
                Assets = await context.Assets.CountAsync(asset => asset.RepositoryId == repositoryId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            };
        }

        public async Task<IPaginatedList<RepositoryDto>> GetAsync(bool showIgnored = false, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Repositories
               .Where(repository => repository.IsIgnored == showIgnored)
               .Select(repository => new RepositoryDto
               {
                   VersionControlId = repository.VersionControlId,
                   Assets = context.Assets.Count(a => a.RepositoryId == repository.Id),
                   Url = repository.Url,
                   WebUrl = repository.WebUrl,
                   RepositoryId = repository.Id,
                   IsIgnored = repository.IsIgnored
               })
               .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByVersionControlIdAsync(Guid versionControlId, bool showIgnored = false, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Repositories
                .Where(repository => repository.IsIgnored == showIgnored)
                .Where(repository => repository.VersionControlId == versionControlId)
                .Select(repository => new RepositoryDto
                {
                    VersionControlId = repository.VersionControlId,
                    Assets = context.Assets.Count(a => a.RepositoryId == repository.Id),
                    Url = repository.Url,
                    WebUrl = repository.WebUrl,
                    RepositoryId = repository.Id,
                    IsIgnored = repository.IsIgnored
                })
                .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByFrameworkIdAsync(Guid frameworkId, bool showIgnored = false, int pageIndex = 1, int pageSize = 10)
        {
            var framework = await context.Frameworks.FindAsync(frameworkId);

            var query = context.Repositories
                .Where(repository => repository.IsIgnored == showIgnored)
                .Where(repository => context.Assets.Any(asset => asset.RepositoryId == repository.Id && context.AssetFrameworks.Any(assetFramework => assetFramework.FrameworkId == frameworkId && assetFramework.AssetId == asset.Id)))
                .Select(repository => new RepositoryDto
                {
                    Assets = context.Assets.Count(asset => asset.RepositoryId == repository.Id),
                    VersionControlId = repository.VersionControlId,
                    WebUrl = repository.WebUrl,
                    Url = repository.Url,
                    RepositoryId = repository.Id,
                    IsIgnored = repository.IsIgnored
                })
                .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IEnumerable<CiCdBuildDto>> GetCiCdsByRepositoryId(Guid repositoryId)
        {
            return await buildsService.GetBuildsByRepositoryIdAsync(repositoryId);
        }
    }
}