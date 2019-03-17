using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IAssetDependencyService
    {
        Task<IPaginatedList<AssetDependencyDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDependencyDto>> GetByDependencyIdAsync(Guid dependencyId, int pageIndex = 1, int pageSize = 10);
    }
}