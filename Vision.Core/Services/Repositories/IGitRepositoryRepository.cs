using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IGitRepositoryRepository
    {
        Task<IEnumerable<GitRepository>> GetAllAsync();
        Task<IEnumerable<GitRepository>> GetBySourceIdAsync(Guid sourceId);
        Task<GitRepository> GetByIdAsync(Guid repositoryId);
        Task SaveAsync(GitRepository repository);
        Task UpdateAsync(GitRepository repository);
        Task DeleteAsync(GitRepository repository);
    }

    public class GitRepositoryRepository : IGitRepositoryRepository
    {
        private readonly VisionDbContext context;

        public GitRepositoryRepository(VisionDbContext context) => this.context = context;

        public async Task DeleteAsync(GitRepository source)
        {
            context.GitRepositories.Remove(source);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GitRepository>> GetAllAsync() => await context.GitRepositories.ToListAsync();

        public async Task<GitRepository> GetByIdAsync(Guid repositoryId) => await context.GitRepositories.FindAsync(repositoryId);

        public async Task<IEnumerable<GitRepository>> GetBySourceIdAsync(Guid sourceId) => await context.GitRepositories.Where(x => x.SourceId == sourceId).ToListAsync();

        public async Task SaveAsync(GitRepository source)
        {
            await context.GitRepositories.AddAsync(source);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GitRepository repository)
        {
            context.GitRepositories.Update(repository);
            await context.SaveChangesAsync();
        }
    }
}
