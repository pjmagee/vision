using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IAssetService
    {
        Task<AssetDto> GetByIdAsync(Guid assetId);
        Task<IPaginatedList<AssetDto>> GetAsync(int pageIndex = 1, int pageSize = 10, DependencyKind? kind = null);
        Task<IPaginatedList<AssetDto>> GetByDependencyIdAsync(Guid dependencyId, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetByFrameworkIdAsync(Guid frameworkId, int pageIndex = 1, int pageSize = 10);        
        Task<IPaginatedList<AssetDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetByVersionControlIdAsync(Guid versionControlId, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetByVersionIdAsync(Guid versionId, int pageIndex = 1, int pageSize = 10);
        Task<IPaginatedList<AssetDto>> GetDependentsByRepositoryId(Guid repositoryId, int pageIndex = 1, int pageSize = 10);
        Task<List<string>> GetPublishedNamesByRepositoryIdAsync(Guid repositoryId);
    }
}