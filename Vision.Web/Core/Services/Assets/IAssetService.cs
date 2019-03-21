using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IAssetService
    {
        Task<AssetDto> GetByIdAsync(Guid id);
        Task<IPaginatedList<AssetDto>> GetAsync(string search, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetByDependencyIdAsync(Guid id, string search, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetByFrameworkIdAsync(Guid id, string search, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetByRepositoryIdAsync(Guid id, string search, IEnumerable<DependencyKind> kinds, bool dependents, int pageIndex = 1, int pageSize = 10);        
        Task<IPaginatedList<AssetDto>> GetByVersionControlIdAsync(Guid id, string search, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetByVersionIdAsync(Guid id, string search, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10);
        Task<List<string>> GetPublishedNamesByRepositoryIdAsync(Guid repositoryId);
    }
}