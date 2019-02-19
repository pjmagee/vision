using System;

namespace Vision.Core
{
    public class AssetDependency : Entity
    {
        public virtual Asset Asset { get; set; }
        public Guid AssetId { get; set; }
        public virtual DependencyVersion DependencyVersion { get; set; }
        public Guid DependencyVersionId { get; set; }
    }
}
