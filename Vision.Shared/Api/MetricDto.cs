namespace Vision.Shared
{
    public class Metric
    {
        public MetricAlertKind Kind { get; set; }
        public MetricCategoryKind CategoryKind { get; set; }
        public DependencyKind? DependencyKind { get; set; }
        public string Title { get; set; }

        public Metric() {  }

        public Metric(MetricAlertKind alertKind, MetricCategoryKind categoryKind, DependencyKind? kind, string title)
        {
            Kind = alertKind;
            CategoryKind = categoryKind;
            DependencyKind = kind;
            Title = title;
        }

        public Metric(MetricAlertKind alertKind, MetricCategoryKind categoryKind, string title)
        {
            Kind = alertKind;
            CategoryKind = categoryKind;
            Title = title;
        }
    }
}
