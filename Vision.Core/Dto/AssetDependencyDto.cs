namespace Vision.Core
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
                EcosystemKind.Maven => Path.GetFileName(asset),
                EcosystemKind.NuGet => Path.GetFileName(asset),
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
        public EcosystemKind Kind { get; set; }
        public Guid VcsId { get; set; }
    }
}
