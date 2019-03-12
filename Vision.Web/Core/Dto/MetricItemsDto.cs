using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class MetricItems<T> : Metric
    {
        public IEnumerable<T> Items { get; set; }

        public MetricItems(MetricAlertKind alert, MetricCategoryKind target, string title, IEnumerable<T> items) : base(alert, target, title)
        {
            Items = items;
        }
    }
}
