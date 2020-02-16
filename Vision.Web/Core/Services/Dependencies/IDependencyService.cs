using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IDependencyService
    {        
        Task<DependencyDto> GetByIdAsync(Guid id);
        Task<IPaginatedList<DependencyDto>> GetAsync(IEnumerable<EcosystemKind> kinds, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<DependencyDto>> GetByRepositoryIdAsync(Guid id, IEnumerable<EcosystemKind> kinds, string search, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<DependencyDto>> GetByAssetIdAsync(Guid id, IEnumerable<EcosystemKind> kinds, string search, int pageIndex = 1, int pageSize = 10);
    }
}