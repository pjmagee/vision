using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public interface IRegistryRepository
    {
        Task<IEnumerable<Registry>> GetAllAsync();
        Task<IEnumerable<Registry>> GetByKindAsync(DependencyKind kind);
        Task<Registry> GetByIdAsync(Guid repositoryId);
        Task SaveAsync(Registry repository);
        Task UpdateAsync(Registry repository);
        Task DeleteAsync(Registry repository);
    }

    public class RegistryRepository : IRegistryRepository
    {
        private readonly VisionDbContext context;

        public RegistryRepository(VisionDbContext context) => this.context = context;

        public async Task DeleteAsync(Registry registry)
        {
            context.Registries.Remove(registry);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Registry>> GetAllAsync() => await context.Registries.ToListAsync();

        public async Task<Registry> GetByIdAsync(Guid registryId) => await context.Registries.FindAsync(registryId);

        public async Task<IEnumerable<Registry>> GetByKindAsync(DependencyKind kind)
        {
            IQueryable<Registry> registries = context.Registries.AsQueryable();

            switch (kind)
            { 
                case DependencyKind.Docker: registries = registries.Where(x => x.IsDocker); break;
                case DependencyKind.NuGet: registries = registries.Where(x => x.IsNuGet); break;
                case DependencyKind.Maven: registries = registries.Where(x => x.IsMaven); break;
                case DependencyKind.Npm: registries = registries.Where(x => x.IsNpm); break;
                case DependencyKind.PyPi: registries = registries.Where(x => x.IsPyPi); break;
                case DependencyKind.RubyGem: registries = registries.Where(x => x.IsRubyGem); break;
            }

            return await registries.ToListAsync();
        }

        public async Task SaveAsync(Registry registry)
        {
            context.Registries.Add(registry);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Registry registry)
        {
            context.Registries.Update(registry);
            await context.SaveChangesAsync();
        }
    }
}
