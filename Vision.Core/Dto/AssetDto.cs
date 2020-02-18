namespace Vision.Core
{
    using System;

    public class AssetDto
    {
        public Guid VcsId { get; set; }
        public Guid AssetId { get; set; }
        public Guid RepositoryId { get; set; }
        public string Asset { get; set; }
        public EcosystemKind Kind { get; set; }
        public string Repository { get; set; }
        public int Dependencies { get; set; }
    }
}
