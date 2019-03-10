using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Framework : Entity
    {
        public string Version { get; set; }
        public virtual ICollection<AssetFramework> Frameworks { get; set; }
    }
}
