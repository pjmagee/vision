namespace Vision.Web.Core
{
    using System;
    using System.IO;

    public class AssetDto
    {
        public Guid VersionControlId { get; set; }
        public Guid AssetId { get; set; }

        public Guid RepositoryId { get; set; }


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

        public DependencyKind Kind { get; set; }
        public string Repository { get; set; }        
        public int Dependencies { get; set; }        
    }
}
