using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> GetAllAsync();
        Task<IEnumerable<Asset>> GetByRepositoryIdAsync(Guid repositoryId);
        Task<Asset> GetByIdAsync(Guid assetId);
        Task SaveAsync(params Asset[] asset);
        Task UpdateAsync(params Asset[] asset);
        Task DeleteAsync(params Asset[] asset);
    }

    public class AssetRepository : IAssetRepository
    {
        private readonly VisionDbContext context;

        public AssetRepository(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await context.Assets.ToListAsync();
        }

        public async Task<IEnumerable<Asset>> GetByRepositoryIdAsync(Guid repositoryId)
        {
            return await context.Assets.Where(a => a.GitRepositoryId == repositoryId).ToListAsync();
        }

        public async Task<Asset> GetByIdAsync(Guid dependencyId)
        {
            return await context.Assets.FindAsync(dependencyId);
        }

        public async Task SaveAsync(params Asset[] dependencies)
        {
            context.Assets.AddRange(dependencies);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(params Asset[] dependencies)
        {
            context.Assets.UpdateRange(dependencies);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(params Asset[] dependencies)
        {
            context.Assets.RemoveRange(dependencies);
            await context.SaveChangesAsync();
        }

        
    }
}
