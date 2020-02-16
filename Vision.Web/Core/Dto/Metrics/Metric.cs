namespace Vision.Web.Core
{
    public class Metric
    {
        public MetricAlertKind Kind { get; set; }
        public CategoryKind CategoryKind { get; set; }
        public EcosystemKind? EcosystemKind { get; set; }
        public string Title { get; set; }

        public Metric() {  }

        public Metric(MetricAlertKind alertKind, CategoryKind categoryKind, EcosystemKind? kind, string title)
        {
            Kind = alertKind;
            CategoryKind = categoryKind;
            EcosystemKind = kind;
            Title = title;
        }

        public Metric(MetricAlertKind alertKind, CategoryKind categoryKind, string title)
        {
            Kind = alertKind;
            CategoryKind = categoryKind;
            Title = title;
        }
    }
}
