using System;

namespace Vision.Shared
{
    public class AssetMetricDto
    {
        public MetricsKind Kind { get; set; }
        public string Title { get; set; }
        public AssetDto[] Items { get; set; }
    }
}
