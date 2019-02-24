using System;

namespace Vision.Shared
{
    public class RepositoryDto
    {
        public Guid VersionControlId { get; set; }
        public Guid RepositoryId { get; set; }
        public string Url { get; set; }
        public string WebUrl { get; set; }
        public int Assets { get; set;  }
    }
}
