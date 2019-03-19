namespace Vision.Web.Core
{
    using System;
    using System.IO;

    public class AssetDto
    {
        private string asset;

        public Guid AssetId { get; set; }
        public string Asset { get => Path.GetFileName(asset); set => asset = value; }
        public DependencyKind Kind { get; set; }
        public string Repository { get; set; }
        public Guid RepositoryId { get; set; }
        public int Dependencies { get; set; }
        public Guid VersionControlId { get; set; }
    }
}
