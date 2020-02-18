using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IRegistryService
    {
        Task<RegistryDto> CreateAsync(RegistryDto dto);
        Task<RegistryDto> GetByIdAsync(Guid registryId);
        Task<RegistryDto> UpdateAsync(RegistryDto dto);
        Task<List<RegistryDto>> GetEnabledByKindAsync(EcosystemKind kind);
        Task<IPaginatedList<RegistryDto>> GetAsync(int pageIndex = 1, int pageSize = 10);
    }
}