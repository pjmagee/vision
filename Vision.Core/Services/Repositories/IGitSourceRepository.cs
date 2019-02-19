using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{

    public interface IGitSourceRepository
    {
        Task<IEnumerable<GitSource>> GetAllAsync();
        Task<GitSource> GetByIdAsync(Guid sourceId);
        Task SaveAsync(GitSource source);
        Task UpdateAsync(GitSource source);
        Task DeleteAsync(GitSource source);
    }

    public class GitSourceRepository : IGitSourceRepository
    {
        private readonly VisionDbContext context;

        public GitSourceRepository(VisionDbContext context) => this.context = context;

        public async Task DeleteAsync(GitSource source)
        {
            context.GitSources.Remove(source);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GitSource>> GetAllAsync() => await context.GitSources.ToListAsync();

        public async Task<GitSource> GetByIdAsync(Guid sourceId) => await context.GitSources.FindAsync(sourceId);

        public async Task SaveAsync(GitSource source)
        {
            context.GitSources.Add(source);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GitSource source)
        {
            context.GitSources.Update(source);
            await context.SaveChangesAsync();
        }
    }
}
