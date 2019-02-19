using System;

namespace Vision.Core
{
    public class AssetFramework : Entity
    {
        public Asset Asset { get; set; }
        public Guid AssetId { get; set; }
        public Framework Framework { get; set; }
        public Guid FrameworkId { get; set; }
    }
}
