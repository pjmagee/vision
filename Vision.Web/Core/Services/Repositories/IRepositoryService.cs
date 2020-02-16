using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IRepositoryService
    {
        Task<RepositoryDto> ToggleIgnoreAsync(Guid repositoryId);
        Task<RepositoryDto> GetByIdAsync(Guid repositoryId);
        Task<IPaginatedList<RepositoryDto>> GetAsync(bool showIgnored, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByEcosystemIdAsync(Guid EcosystemId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByEcosystemVersionIdAsync(Guid EcosystemVersionId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByVcsIdAsync(Guid VcsId, bool showIgnored, string search, int pageIndex = 1, int pageSize = 10);
    }
}