using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class DependencyVersion : Entity
    {
        public string Version { get; set; }
        public string VulnerabilityUrl { get; set; }
        public bool IsLatest { get; set; }
        public virtual Dependency Dependency { get; set; }
        public Guid DependencyId { get; set; }

        public virtual ICollection<AssetDependency> Assets { get; set; }

        public DependencyVersion()
        {
            Id = Guid.NewGuid();
        }
    }
}
