using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Framework : Entity
    {
        public string Name { get; set; } // Node, .NET, JDK, NPM
        public string Version { get; set; } // NPM Version, netcore2.0, jdk7.0 etc
        public virtual ICollection<AssetFramework> Frameworks { get; set; }
    }
}
