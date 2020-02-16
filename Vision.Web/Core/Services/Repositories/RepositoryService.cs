namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class RepositoryService : IRepositoryService
    {
        private readonly VisionDbContext context;

        public RepositoryService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<RepositoryDto> ToggleIgnoreAsync(Guid repositoryId)
        {
            VcsRepository repository = await context.Repositories.FindAsync(repositoryId);
            repository.IsIgnored = !repository.IsIgnored;
            await context.SaveChangesAsync();

            if (repository.IsIgnored)
            {
                context.RemoveRange(context.Assets.Where(asset => asset.RepositoryId == repositoryId));
            }
            else
            {
                context.Tasks.Add(new RefreshTask { Scope = TaskScopeKind.Repository, TargetId = repositoryId });
            }

            await context.SaveChangesAsync();

            return new RepositoryDto
            {
                VersionControlId = repository.VcsId,
                Assets = context.Assets.Count(asset => asset.RepositoryId == repositoryId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            };
        }

        public async Task<RepositoryDto> GetByIdAsync(Guid repositoryId)
        {
            VcsRepository repository = await context.Repositories.FindAsync(repositoryId);

            return new RepositoryDto
            {
                VersionControlId = repository.VcsId,
                Assets = context.Assets.Count(asset => asset.RepositoryId == repositoryId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            };
        }

        public async Task<IPaginatedList<RepositoryDto>> GetAsync(bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Repositories.Where(repository => repository.IsIgnored == showIgnored);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(repository => repository.WebUrl.Contains(search) || repository.Url.Contains(search));
            }

            var paging = query.Select(repository => new RepositoryDto
            {
                VersionControlId = repository.VcsId,
                Assets = context.Assets.Count(a => a.RepositoryId == repository.Id),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            })
            .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByVersionControlIdAsync(Guid versionControlId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Repositories.Where(repository => repository.IsIgnored == showIgnored).Where(repository => repository.VcsId == versionControlId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(repository => repository.WebUrl.Contains(search) || repository.Url.Contains(search));
            }

            var paging = query.Select(repository => new RepositoryDto
            {
                VersionControlId = repository.VcsId,
                Assets = context.Assets.Count(a => a.RepositoryId == repository.Id),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            })
            .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByRuntimeIdAsync(Guid runtimeId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            Runtime runtime = await context.Runtimes.FindAsync(runtimeId);

            IQueryable<VcsRepository> repositories = context.Repositories
                .Where(repository => repository.IsIgnored == showIgnored)
                .Where(repository => context.Assets.Any(asset => asset.RepositoryId == repository.Id &&
                                                                 context.AssetRuntimes.Any(ar => ar.RuntimeVersion.RuntimeId == runtimeId &&
                                                                                                 ar.AssetId == asset.Id)));

            var query = repositories.Select(repository => new RepositoryDto
            {
                Assets = context.Assets.Count(asset => asset.RepositoryId == repository.Id),
                VersionControlId = repository.VcsId,
                WebUrl = repository.WebUrl,
                Url = repository.Url,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            })
            .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByRuntimeVersionIdAsync(Guid runtimeVersionId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            RuntimeVersion version = await context.RuntimeVersions.FindAsync(runtimeVersionId);

            IQueryable<VcsRepository> repositories = context.Repositories
                .Where(repository => repository.IsIgnored == showIgnored)
                .Where(repository => context.Assets.Any(asset => asset.RepositoryId == repository.Id &&
                                                                 context.AssetRuntimes.Any(ar => ar.RuntimeVersionId == runtimeVersionId &&
                                                                                                 ar.AssetId == asset.Id)));

            var query = repositories.Select(repository => new RepositoryDto
            {
                Assets = context.Assets.Count(asset => asset.RepositoryId == repository.Id),
                VersionControlId = repository.VcsId,
                WebUrl = repository.WebUrl,
                Url = repository.Url,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            })
            .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}