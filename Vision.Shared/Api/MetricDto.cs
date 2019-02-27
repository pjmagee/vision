using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision.Shared
{
    public class Metric
    {
        public MetricKind Kind { get; set; }
        public MetricCategoryKind CategoryKind { get; set; }
        public DependencyKind? DependencyKind { get; set; }
        public string Title { get; set; }

        public Metric() {  }

        public Metric(MetricKind alert, MetricCategoryKind target, DependencyKind? kind, string title)
        {
            Kind = alert;
            CategoryKind = target;
            DependencyKind = kind;
            Title = title;
        }

        public Metric(MetricKind alert, MetricCategoryKind target, string title)
        {
            Kind = alert;
            CategoryKind = target;
            Title = title;
        }
    }
}
