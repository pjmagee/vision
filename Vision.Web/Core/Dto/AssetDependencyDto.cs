namespace Vision.Web.Core
{
    using System;
    using System.IO;

    public class AssetDependencyDto
    {
        private string asset;

        public string Asset
        {
            get => Kind switch
            {
                DependencyKind.Maven => Path.GetFileName(asset),
                DependencyKind.NuGet => Path.GetFileName(asset),
                _ => asset
            };
            set => asset = value;
        }

        public string Repository { get; set; }
        public string Dependency { get; set; }
        public string Version { get; set; }
        public bool IsLatest { get; set; }
        public Guid AssetId { get; set; }
        public Guid DependencyId { get; set; }
        public Guid DependencyVersionId { get; set; }
        public Guid RepositoryId { get; set; }
        public DependencyKind Kind { get; set; }
        public Guid VersionControlId { get; set; }
    }
}
