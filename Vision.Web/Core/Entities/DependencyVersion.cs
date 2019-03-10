using System;
using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class DependencyVersion : Entity, IComparable<DependencyVersion>
    {
        public string Version { get; set; }
        public bool IsLatest { get; set; }
        public virtual Dependency Dependency { get; set; }
        public Guid DependencyId { get; set; }
        public virtual ICollection<AssetDependency> Assets { get; set; }
        
        public int CompareTo(DependencyVersion other)
        {
            // this does NOT support semantic versioning AT ALL.
            // quick fix

            return new Version(Version).CompareTo(new Version(other.Version));
        }
    }
}
