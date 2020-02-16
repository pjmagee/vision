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

        public async Task<RepositoryDto> ToggleIgnoreAsync(Guid repoId)
        {
            VcsRepository repository = await context.VcsRepositories.FindAsync(repoId);
            repository.IsIgnored = !repository.IsIgnored;
            await context.SaveChangesAsync();

            if (repository.IsIgnored)
            {
                context.RemoveRange(context.Assets.Where(asset => asset.RepositoryId == repoId));
            }
            else
            {
                context.Tasks.Add(new RefreshTask { Scope = TaskScopeKind.Repository, TargetId = repoId });
            }

            await context.SaveChangesAsync();

            return new RepositoryDto
            {
                VcsId = repository.VcsId,
                Assets = context.Assets.Count(asset => asset.RepositoryId == repoId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            };
        }

        public async Task<RepositoryDto> GetByIdAsync(Guid repoId)
        {
            VcsRepository repository = await context.VcsRepositories.FindAsync(repoId);

            return new RepositoryDto
            {
                VcsId = repository.VcsId,
                Assets = context.Assets.Count(asset => asset.RepositoryId == repoId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            };
        }

        public async Task<IPaginatedList<RepositoryDto>> GetAsync(bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.VcsRepositories.Where(repository => repository.IsIgnored == showIgnored);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(repository => repository.WebUrl.Contains(search) || repository.Url.Contains(search));
            }

            var paging = query.Select(repository => new RepositoryDto
            {
                VcsId = repository.VcsId,
                Assets = context.Assets.Count(a => a.RepositoryId == repository.Id),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            })
            .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByVcsIdAsync(Guid vcsId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.VcsRepositories.Where(repository => repository.IsIgnored == showIgnored).Where(repository => repository.VcsId == vcsId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(repository => repository.WebUrl.Contains(search) || repository.Url.Contains(search));
            }

            var paging = query.Select(repository => new RepositoryDto
            {
                VcsId = repository.VcsId,
                Assets = context.Assets.Count(a => a.RepositoryId == repository.Id),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            })
            .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(paging, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByEcosystemIdAsync(Guid ecoId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<VcsRepository> repositories = context.VcsRepositories
                .Where(repository => repository.IsIgnored == showIgnored)
                .Where(repository => context.Assets.Any(asset => asset.RepositoryId == repository.Id &&
                                                                 context.AssetEcoSystems.Any(ar => ar.EcosystemVersion.EcosystemId == ecoId &&
                                                                                                 ar.AssetId == asset.Id)));

            var query = repositories.Select(repository => new RepositoryDto
            {
                Assets = context.Assets.Count(asset => asset.RepositoryId == repository.Id),
                VcsId = repository.VcsId,
                WebUrl = repository.WebUrl,
                Url = repository.Url,
                RepositoryId = repository.Id,
                IsIgnored = repository.IsIgnored
            })
            .OrderByDescending(r => r.Assets);

            return await PaginatedList<RepositoryDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RepositoryDto>> GetByEcosystemVersionIdAsync(Guid ecoVerId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<VcsRepository> repositories = context.VcsRepositories
                .Where(repository => repository.IsIgnored == showIgnored)
                .Where(repository => context.Assets.Any(asset => asset.RepositoryId == repository.Id &&
                                                                 context.AssetEcoSystems.Any(ar => ar.EcosystemVersionId == ecoVerId &&
                                                                                                 ar.AssetId == asset.Id)));

            var query = repositories.Select(repository => new RepositoryDto
            {
                Assets = context.Assets.Count(asset => asset.RepositoryId == repository.Id),
                VcsId = repository.VcsId,
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