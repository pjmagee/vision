using System;

namespace Vision.Shared
{
    public class AssetDto
    {
        public Guid AssetId { get; set; }
        public string Asset { get; set; }
        public string Repository { get; set; }
        public Guid RepositoryId { get; set; }
        public int Dependencies { get; set; }
    }
}
