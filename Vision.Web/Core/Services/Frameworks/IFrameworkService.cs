using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IFrameworkService
    {
        Task<FrameworkDto> GetByIdAsync(Guid frameworkId);
        Task<IPaginatedList<FrameworkDto>> GetAsync(int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<FrameworkDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10);        
        Task<IPaginatedList<FrameworkDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10);
    }
}