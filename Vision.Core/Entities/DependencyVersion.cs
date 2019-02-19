using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class DependencyVersion : Entity
    {
        public string Version { get; set; }
        public bool IsVulnerable { get; set; }
        public string VulnerabilityUrl { get; set; }
        public Dependency Dependency { get; set; }
        public Guid DependencyId { get; set; }
        public List<AssetDependency> Assets { get; set; }
    }
}
