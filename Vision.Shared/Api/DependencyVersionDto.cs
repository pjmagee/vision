using System;

namespace Vision.Shared
{
    public class DependencyVersionDto
    {
        public int Assets { get; set; }
        public Guid DependencyVersionId { get; set;  }
        public string Version { get; set; }
        public bool IsLatest { get; set; }        
        public Guid DependencyId { get; set; }
    }
}
