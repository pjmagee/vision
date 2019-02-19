using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IDependencyVersionRepository
    {
        Task<DependencyVersion> GetByIdAsync(Guid dependencyId);
        Task SaveAsync(params DependencyVersion[] dependency);
        Task UpdateAsync(params DependencyVersion[] dependency);
        Task DeleteAsync(params DependencyVersion[] dependency);
        Task<IEnumerable<DependencyVersion>> GetVersionsByDependencyIdAsync(Guid dependencyId);
    }

    public class DependencyVersionRepository : IDependencyVersionRepository
    {
        private readonly VisionDbContext context;

        public DependencyVersionRepository(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task DeleteAsync(params DependencyVersion[] versions)
        {
            context.DependencyVersions.RemoveRange(versions);
            await context.SaveChangesAsync();
        }

        public async Task<DependencyVersion> GetByIdAsync(Guid versionId)
        {
            return await context.DependencyVersions.FindAsync(versionId);
        }

        public async Task<IEnumerable<DependencyVersion>> GetVersionsByDependencyIdAsync(Guid dependencyId)
        {
            return await context.DependencyVersions.Where(x => x.DependencyId == dependencyId).ToListAsync();
        }

        public async Task SaveAsync(params DependencyVersion[] versions)
        {
            context.DependencyVersions.AddRange(versions);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(params DependencyVersion[] versions)
        {
            context.DependencyVersions.UpdateRange(versions);
            await context.SaveChangesAsync();
        }
    }
}
