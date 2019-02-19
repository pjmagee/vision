using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{

    public interface IDependencyRepository
    {
        Task<IEnumerable<Dependency>> GetAllAsync();
        Task<Dependency> GetByIdAsync(Guid dependencyId);
        Task SaveAsync(params Dependency[] dependency);
        Task UpdateAsync(params Dependency[] dependency);
        Task DeleteAsync(params Dependency[] dependency);        
    }    

    public class DependencyRepository : IDependencyRepository
    {
        private readonly VisionDbContext context;

        public DependencyRepository(VisionDbContext context)
        {
            this.context = context;
        }        

        public async Task<IEnumerable<Dependency>> GetAllAsync()
        {
            return await context.Dependencies.ToListAsync();
        }

        public async Task<Dependency> GetByIdAsync(Guid dependencyId)
        {
            return await context.Dependencies.FindAsync(dependencyId);
        }

        public async Task SaveAsync(params Dependency[] dependencies)
        {
            context.Dependencies.AddRange(dependencies);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(params Dependency[] dependencies)
        {
            context.Dependencies.UpdateRange(dependencies);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(params Dependency[] dependencies)
        {
            context.Dependencies.RemoveRange(dependencies);
            await context.SaveChangesAsync();
        }
    }
}
