namespace Vision.Web.Core
{
    using System.Collections.Generic;

    public class MetricItems<T> : Metric
    {
        public IEnumerable<T> Items { get; set; }

        public MetricItems(MetricAlertKind alert, CategoryKind target, string title, IEnumerable<T> items) : base(alert, target, title)
        {
            Items = items;
        }
    }
}
