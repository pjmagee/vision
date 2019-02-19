using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class Asset : Entity
    {
        public string Path { get; set; }
        public string Raw { get; set; }
        public virtual GitRepository GitRepository { get; set; }
        public Guid GitRepositoryId { get; set; }
        public virtual IList<AssetDependency> Dependencies { get; set; }
        public virtual IList<AssetFramework> Frameworks { get; set; }
    }
}
