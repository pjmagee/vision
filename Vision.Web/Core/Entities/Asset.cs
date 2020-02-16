using System;
using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Asset : Entity
    {
        public Guid RepositoryId { get; set; }
        public Guid AssetRuntimeId { get; set; }

        public string Path { get; set; }

        public string Raw { get; set; }

        public DependencyKind Kind { get; set; }

        public virtual VcsRepository Repository { get; set; }
        public virtual AssetRuntime AssetRuntime { get; set; }
        public virtual ICollection<AssetDependency> Dependencies { get; set; }
    }
}
