using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IRuntimeService
    {
        Task<RuntimeDto> GetRuntimeByIdAsync(Guid runtimeId);

        Task<RuntimeVersionDto> GetVersionByIdAsync(Guid runtimeVersionId);

        Task<IPaginatedList<RuntimeDto>> GetAsync(int pageIndex = 1, int pageSize = 10);

        Task<IPaginatedList<RuntimeDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10);

        Task<IPaginatedList<RuntimeDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10);

        Task<IPaginatedList<RuntimeVersionDto>> GetByRuntimeIdAsync(Guid runtimeId, int pageIndex = 1, int pageSize = 10);
    }
}