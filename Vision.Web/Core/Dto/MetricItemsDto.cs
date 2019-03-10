namespace Vision.Web.Core
{
    public class MetricItems<T> : Metric
    {
        public T[] Items { get; set; }

        public MetricItems(MetricAlertKind alert, MetricCategoryKind target, string title, T[] items) : base(alert, target, title)
        {
            Items = items;
        }
    }
}
