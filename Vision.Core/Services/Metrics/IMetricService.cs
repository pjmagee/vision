using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IMetricService
    {
        Task<IEnumerable<MetricItem>> GetCountsAsync();
        Task<IEnumerable<MetricItem>> GetMetricsAsync(Guid id, CategoryKind targetKind);
    }
}