namespace Vision.Shared
{
    public class MetricItems<T> : Metric
    {
        public T[] Items { get; set; }

        public MetricItems(MetricKind alert, MetricCategoryKind target, string title, T[] items) : base(alert, target, title)
        {
            Items = items;
        }
    }
}
