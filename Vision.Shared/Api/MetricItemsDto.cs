namespace Vision.Shared
{
    public class MetricItems<T> : Metric
    {
        public T[] Items { get; set; }

        public MetricItems(MetricsKind kind, string title, T[] items) : base(kind, title)
        {
            Items = items;
        }
    }
}
