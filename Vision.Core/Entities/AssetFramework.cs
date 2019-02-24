using System;

namespace Vision.Core
{
    public class AssetFramework : Entity, IEquatable<AssetFramework>
    {
        public virtual Asset Asset { get; set; }
        public Guid AssetId { get; set; }
        public virtual Framework Framework { get; set; }
        public Guid FrameworkId { get; set; }

        public bool Equals(AssetFramework other) => Id.Equals(other.Id);
    }
}
