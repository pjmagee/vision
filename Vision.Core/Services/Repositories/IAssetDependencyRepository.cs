using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task DeleteAsync(params AssetDependency[] assetDependencies)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AssetDependency>> GetByAssetIdAsync(Guid assetId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AssetDependency>> GetByDependencyIdAsync(Guid dependencyId)
        {
            throw new NotImplementedException();
        }

        public async Task<AssetDependency> GetByIdAsync(Guid assetDependencyId)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync(params AssetDependency[] assetDependencies)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(params AssetDependency[] assetDependencies)
        {
            throw new NotImplementedException();
        }
    }
}
