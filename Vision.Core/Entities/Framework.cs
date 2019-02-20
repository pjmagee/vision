using System.Collections.Generic;
using Vision.Shared;

namespace Vision.Core
{
    public class Framework : Entity
    {
        public string Version { get; set; }

        public virtual ICollection<AssetFramework> Frameworks { get; set; }
    }
}
