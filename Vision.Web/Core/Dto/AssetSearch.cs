namespace Vision.Web.Core
{
    using System;

    public class AssetSearch
    {
        public string Name { get; set; }
        public DependencyKind[] Kinds { get; set; }
        public Guid[] VersionControlIds { get; set; }
        public Guid[] DependencyIds { get; set; }
    }
}
