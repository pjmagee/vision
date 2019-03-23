using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IMetricService
    {
        Task<IEnumerable<MetricItem>> GetCountsAsync();
        Task<IEnumerable<MetricItem>> GetMetricsAsync(Guid id, MetricCategoryKind targetKind);
    }
}