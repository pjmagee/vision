using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class Asset : Entity
    {
        public string Path { get; set; }
        public string Raw { get; set; }
        public GitRepository Repository { get; set; }
        public Guid RepositoryId { get; set; }
        public List<AssetDependency> Dependencies { get; set; }
        public List<AssetFramework> Frameworks { get; set; }
    }
}
