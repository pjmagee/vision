using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IDependencyService
    {        
        Task<DependencyDto> GetByIdAsync(Guid dependencyId);
        Task<IPaginatedList<DependencyDto>> GetAsync(IEnumerable<DependencyKind> kinds, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<DependencyDto>> GetByRepositoryIdAsync(Guid repositoryId, IEnumerable<DependencyKind> kinds, string search, int pageIndex = 1, int pageSize = 10);
    }
}