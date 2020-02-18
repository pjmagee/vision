using System.Collections.Generic;

namespace Vision.Core
{
    public class Ecosystem : Entity
    {
        public string Name { get; set; } // Node, .NET, Java, Ruby, Python
        public virtual ICollection<EcosystemVersion> EcosystemVersions { get; set; }
    }
}
