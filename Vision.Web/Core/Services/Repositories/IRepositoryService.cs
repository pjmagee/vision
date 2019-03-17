using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IRepositoryService
    {               
        Task<RepositoryDto> ToggleIgnoreAsync(Guid repositoryId);
        Task<RepositoryDto> GetByIdAsync(Guid repositoryId);
        Task<IPaginatedList<RepositoryDto>> GetAsync(bool showIgnored = false, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByFrameworkIdAsync(Guid frameworkId, bool showIgnored = false, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<RepositoryDto>> GetByVersionControlIdAsync(Guid versionControlId, bool showIgnored = false, int pageIndex = 1, int pageSize = 10);
    }
}