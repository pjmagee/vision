using System;

namespace Vision.Shared
{
    public class AssetSearch
    {
        public string Name { get; set; }
        public DependencyKind Kind { get; set; }
        public Guid[] GitSourcIds { get; set; }
        public Guid[] DependencyIds { get; set; }
    }
}
