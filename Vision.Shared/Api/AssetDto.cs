using System;

namespace Vision.Shared
{
    public class AssetDto
    {
        public Guid AssetId { get; set; }
        public string Path { get; set; }
        public Guid RepositoryId { get; set; }
        public int Dependencies { get; set; }
    }
}
