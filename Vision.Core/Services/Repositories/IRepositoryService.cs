using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IRepositoryService
    {
        Task<RepositoryDto> ToggleIgnoreAsync(Guid repositoryId);
        Task<RepositoryDto> GetByIdAsync(Guid repositoryId);
        Task<IPaginatedList<RepositoryDto>> GetAsync(string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByEcosystemIdAsync(Guid EcosystemId, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByEcosystemVersionIdAsync(Guid EcosystemVersionId, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByVcsIdAsync(Guid VcsId, string search, int pageIndex = 1, int pageSize = 10);
    }
}