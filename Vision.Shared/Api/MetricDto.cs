using System.Collections.Generic;
using System.Linq;

namespace Vision.Shared
{
    public class Metric
    {
        public MetricsKind Kind { get; set; }
        public string Title { get; set; }

        public Metric(MetricsKind kind, string title)
        {
            Kind = kind;
            Title = title;
        }
    }
}
