using System;

namespace Vision.Web.Core
{
    public class AssetRuntime : Entity, IEquatable<AssetRuntime>
    {
        public Guid AssetId { get; set; }
        public Guid RuntimeVersionId { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual RuntimeVersion RuntimeVersion { get; set; }

        public bool Equals(AssetRuntime other) => Id.Equals(other.Id);
    }
}
