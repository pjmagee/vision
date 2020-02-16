using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IEcosystemService
    {
        Task<EcosystemDto> GetEcosystemByIdAsync(Guid EcosystemId);

        Task<EcosystemVersionDto> GetEcoVersionByIdAsync(Guid EcosystemVersionId);

        Task<IPaginatedList<EcosystemDto>> GetAsync(int pageIndex = 1, int pageSize = 10);

        Task<IPaginatedList<EcosystemDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10);

        Task<IPaginatedList<EcosystemDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10);

        Task<IPaginatedList<EcosystemVersionDto>> GetByEcosystemIdAsync(Guid EcosystemId, int pageIndex = 1, int pageSize = 10);
    }
}