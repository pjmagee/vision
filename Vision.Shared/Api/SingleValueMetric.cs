namespace Vision.Shared
{
    public class MetricItem : Metric
    {
        public string Value { get; set; }

        public MetricItem(MetricsKind kind, string title, object value) : base(kind, title)
        {
            Value = value.ToString();
        }
    }
}
