namespace Vision.Shared
{
    public class MetricItem : Metric
    {
        public string Value { get; set; }

        public MetricItem() {  }

        public MetricItem(MetricKind alert, MetricCategoryKind target, string title, object value) : base(alert, target, null, title)
        {
            Value = value.ToString();
        }

        public MetricItem(MetricKind alert, MetricCategoryKind target, DependencyKind? kind, string title, object value) : base(alert, target, kind, title)
        {
            Value = value.ToString();
        }
    }
}
