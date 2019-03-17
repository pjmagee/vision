namespace Vision.Web.Core
{
    using System;
    using System.Threading.Tasks;

    public interface IRefreshService
    {
        Task RefreshAllAsync();
        Task RefreshVersionControlByIdAsync(Guid sourceId);
        Task RefreshRepositoryByIdAsync(Guid repositoryId);
        Task RefreshAssetByIdAsync(Guid assetId);
        Task RefreshAssetDependenciesAsync(Guid assetId);
        Task RefreshAssetFrameworksAsync(Guid assetId);
        Task RefreshDependencyByIdAsync(Guid dependencyId);
    }
}
