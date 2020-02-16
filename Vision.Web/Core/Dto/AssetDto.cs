namespace Vision.Web.Core
{
    using System;
    using System.IO;

    public class AssetDto
    {
        public Guid VcsId { get; set; }
        public Guid AssetId { get; set; }

        public Guid RepositoryId { get; set; }


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

        public EcosystemKind Kind { get; set; }
        public string Repository { get; set; }        
        public int Dependencies { get; set; }        
    }
}
