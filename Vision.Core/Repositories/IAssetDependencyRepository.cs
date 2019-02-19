using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Vision.Core
{
    public interface IAssetDependencyRepository
    {
        Task<IEnumerable<AssetDependency>> GetByAssetIdAsync(Guid assetId);
        Task<IEnumerable<AssetDependency>> GetByDependencyIdAsync(Guid dependencyId);
        Task<AssetDependency> GetByIdAsync(Guid assetDependencyId);
        Task SaveAsync(params AssetDependency[] assetDependencies);
        Task UpdateAsync(params AssetDependency[] assetDependencies);
        Task DeleteAsync(params AssetDependency[] assetDependencies);
    }

    public class AssetDependencyRepository : IAssetDependencyRepository
    {
        private readonly VisionDbContext context;

        public AssetDependencyRepository(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<AssetDependency>> GetByAssetIdAsync(Guid assetId)
        {
            return await context.AssetDependencies.Where(ad => ad.AssetId == assetId).ToListAsync();
        }

        public async Task<IEnumerable<AssetDependency>> GetByDependencyIdAsync(Guid dependencyId)
        {
            List<Guid> dependencyIds = await context.DependencyVersions.Where(v => v.DependencyId == dependencyId).Select(x => x.DependencyId).Distinct().ToListAsync();
            return await context.AssetDependencies.Where(ad => dependencyIds.Contains(ad.DependencyVersionId)).ToListAsync();
        }

        public async Task<AssetDependency> GetByIdAsync(Guid assetDependencyId)
        {
            return await context.AssetDependencies.FindAsync(assetDependencyId);
        }

        public async Task SaveAsync(params AssetDependency[] assetDependencies)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(params AssetDependency[] assetDependencies)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteAsync(params AssetDependency[] assetDependencies)
        {
            throw new NotImplementedException();
        }
    }
}
