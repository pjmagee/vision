using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IDependencyService
    {        
        Task<DependencyDto> GetByIdAsync(Guid dependencyId);

        Task<IPaginatedList<DependencyDto>> GetAsync(int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<DependencyDto>> GetByKindsAsync(DependencyKind[] kinds, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<DependencyDto>> GetByRegistryIdAsync(Guid registryId, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<DependencyDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10);
    }
}