using System;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IDependencyVersionService
    {
        Task<IPaginatedList<DependencyVersionDto>> GetByDependencyIdAsync(Guid dependencyId, int pageIndex = 1, int pageSize = 10);
        Task<DependencyVersionDto> GetByIdAsync(Guid dependencyVersionId);
    }
}