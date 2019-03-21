using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IAssetService
    {
        Task<AssetDto> GetByIdAsync(Guid assetId);
        Task<IPaginatedList<AssetDto>> GetAsync(IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null);
        Task<IPaginatedList<AssetDto>> GetByDependencyIdAsync(Guid dependencyId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null);
        Task<IPaginatedList<AssetDto>> GetByFrameworkIdAsync(Guid frameworkId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null);
        Task<IPaginatedList<AssetDto>> GetByRepositoryIdAsync(Guid repositoryId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null);
        Task<IPaginatedList<AssetDto>> GetByVersionControlIdAsync(Guid versionControlId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null);
        Task<IPaginatedList<AssetDto>> GetByVersionIdAsync(Guid versionId, IEnumerable<DependencyKind> kinds, int pageIndex = 1, int pageSize = 10, string search = null);
        Task<List<string>> GetPublishedNamesByRepositoryIdAsync(Guid repositoryId);
    }
}