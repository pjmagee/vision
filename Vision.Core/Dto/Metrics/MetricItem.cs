namespace Vision.Core
{
    public class MetricItem : Metric
    {
        public string Value { get; set; }

        public MetricItem() {  }

        public MetricItem(MetricAlertKind alert, CategoryKind target, string title, object value) : base(alert, target, null, title)
        {
            Value = value.ToString();
        }

        public MetricItem(MetricAlertKind alert, CategoryKind target, EcosystemKind? kind, string title, object value) : base(alert, target, kind, title)
        {
            Value = value.ToString();
        }
    }
}
