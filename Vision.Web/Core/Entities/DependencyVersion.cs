using System;
using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class DependencyVersion : Entity, IComparable<DependencyVersion>
    {
        public string Version { get; set; }
        public string ProjectUrl { get; set; }
        public bool IsLatest { get; set; }
        public virtual Dependency Dependency { get; set; }
        public Guid DependencyId { get; set; }
        public virtual ICollection<AssetDependency> Assets { get; set; }

        public int CompareTo(DependencyVersion other)
        {
            // quick hack - this does NOT support semantic versioning AT ALL.

            if (System.Version.TryParse(Version, out Version? v1))
            {
                if (System.Version.TryParse(other.Version, out Version? v2))
                {
                    return v1.CompareTo(v2);
                }
            }

            return Version.CompareTo(other.Version);
        }
    }
}
