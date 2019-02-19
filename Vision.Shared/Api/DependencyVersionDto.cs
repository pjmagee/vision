using System;

namespace Vision.Shared.Api
{
    public class DependencyVersionDto
    {
        public Guid DependencyVersionId { get; set;  }
        public string Version { get; set; }
        public bool IsLatest { get; set; }
        public bool IsVulnerable { get; set; }
        public string VulnerabilityUrl { get; set; }        
        public Guid DependencyId { get; set; }
    }
}
