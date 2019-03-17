using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IRegistryService
    {
        Task<RegistryDto> CreateAsync(RegistryDto dto);        
        Task<RegistryDto> GetByIdAsync(Guid registryId);
        Task<RegistryDto> UpdateAsync(RegistryDto dto);

        Task<IPaginatedList<RegistryDto>> GetAsync(int pageIndex = 1, int pageSize = 10);
    }
}