using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IMetricService
    {
        Task<IEnumerable<MetricItems<AssetDto>>> GetAssetInsightsByKindAsync(DependencyKind dependencyKind);
        Task<IEnumerable<MetricItem>> GetCountsAsync();
        Task<IEnumerable<MetricItems<DependencyDto>>> GetDependencyInsightsByKindAsync(DependencyKind dependencyKind);
        Task<IEnumerable<MetricItems<RepositoryDto>>> GetPublishingRepositoriesByDependencyKind(DependencyKind dependencyKind);
        Task<IEnumerable<MetricItems<RepositoryDto>>> GetRepositoriesMetricsByVersionControlIdAsync(Guid versionControlId);
    }
}