using System;
using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Dependency : Entity
    {
        public string Name { get; set; }
        public DependencyKind Kind { get; set; }
        public DateTime Updated { get; set; }
        public virtual ICollection<AssetDependency> Assets { get; set; }
        public virtual ICollection<DependencyVersion> Versions { get; set; }
    }
}
