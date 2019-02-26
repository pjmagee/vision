using System.Collections.Generic;
using System.Linq;

namespace Vision.Shared
{
    public class MetricDto
    {
        public MetricsKind Kind { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }

        public MetricDto(MetricsKind kind, string title, object value)
        {
            Kind = kind;
            Title = title;
            Value = value.ToString();
        }
    }
}
