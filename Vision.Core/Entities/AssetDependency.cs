using System;

namespace Vision.Core
{
    public class AssetDependency : Entity
    {
        public Asset Asset { get; set; }
        public Guid AssetId { get; set; }
        public DependencyVersion DependencyVersion { get; set; }
        public Guid DependencyVersionId { get; set; }
    }
}
