using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IDependencyVersionService
    {
        Task<IPaginatedList<DependencyVersionDto>> GetByDependencyIdAsync(Guid dependencyId, int pageIndex = 1, int pageSize = 10);
        Task<DependencyVersionDto> GetByIdAsync(Guid dependencyVersionId);
    }
}