using System;

namespace Vision.Core
{
    public class AssetFramework : Entity
    {
        public virtual Asset Asset { get; set; }
        public Guid AssetId { get; set; }
        public virtual Framework Framework { get; set; }
        public Guid FrameworkId { get; set; }
    }
}
