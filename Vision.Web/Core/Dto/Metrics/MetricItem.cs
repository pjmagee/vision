namespace Vision.Web.Core
{
    public class MetricItem : Metric
    {
        public string Value { get; set; }

        public MetricItem() {  }

        public MetricItem(MetricAlertKind alert, MetricCategoryKind target, string title, object value) : base(alert, target, null, title)
        {
            Value = value.ToString();
        }

        public MetricItem(MetricAlertKind alert, MetricCategoryKind target, DependencyKind? kind, string title, object value) : base(alert, target, kind, title)
        {
            Value = value.ToString();
        }
    }
}
