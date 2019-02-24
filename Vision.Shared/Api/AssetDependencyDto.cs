using System;

namespace Vision.Shared
{
    public class AssetDependencyDto
    {
        public string Asset { get; set; }
        public string Repository { get; set; }
        public string Dependency { get; set; }
        public string Version { get; set; }
        public bool IsLatest { get; set; }
        public Guid AssetId { get; set; }
        public Guid DependencyId { get; set; }
        public Guid DependencyVersionId { get; set; }
        public Guid RepositoryId { get; set; }
    }
}
