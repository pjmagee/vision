namespace Vision.Web.Core
{
    using System;

    public class AssetDto
    {
        public Guid AssetId { get; set; }
        public string Asset { get; set; }
        public DependencyKind Kind { get; set; }
        public string Repository { get; set; }
        public Guid RepositoryId { get; set; }
        public int Dependencies { get; set; }
        public Guid VersionControlId { get; set; }
    }
}
