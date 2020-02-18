using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class Asset : Entity
    {
        public Guid RepositoryId { get; set; }

        public Guid AssetEcosystemId { get; set; }

        public string Path { get; set; }

        public string Raw { get; set; }

        public EcosystemKind Kind { get; set; }

        public virtual VcsRepository Repository { get; set; }

        public virtual AssetEcosystem AssetEcosystem { get; set; }

        public virtual ICollection<AssetDependency> Dependencies { get; set; }
    }
}
