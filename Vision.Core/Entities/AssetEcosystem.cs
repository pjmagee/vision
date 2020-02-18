using System;

namespace Vision.Core
{
    public class AssetEcosystem : Entity
    {
        public Guid AssetId { get; set; }
        public Guid EcosystemVersionId { get; set; }
        public virtual Asset Asset { get; set; }
        public virtual EcosystemVersion EcosystemVersion { get; set; }
    }
}
