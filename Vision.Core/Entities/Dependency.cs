using System;
using System.Collections.Generic;
using Vision.Shared;

namespace Vision.Core
{
    public class Dependency : Entity
    {
        public string Name { get; set; }
        public string RepositoryUrl { get; set; }
        public DependencyKind Kind { get; set; }
        public DateTime Updated { get; set; }
        public Registry Registry { get; set; }
        public Guid RegistryId { get; set; }
        public List<AssetDependency> Assets { get; set; }
        public List<DependencyVersion> Versions { get; set; }
    }
}
