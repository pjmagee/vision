using System;

namespace Vision.Core
{
    public class AssetDependency : Entity
    {
        public Guid AssetId { get; set; }
        public Guid DependencyId { get; set; }
        public Guid DependencyVersionId { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual DependencyVersion DependencyVersion { get; set; }
        public virtual Dependency Dependency { get; set; }
    }
}
