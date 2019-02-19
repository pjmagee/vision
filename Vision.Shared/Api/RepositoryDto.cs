using System;

namespace Vision.Shared
{
    public class RepositoryDto
    {
        public Guid SourceId { get; set; }
        public Guid RepositoryId { get; set; }
        public string GitUrl { get; set; }
        public string WebUrl { get; set; }
        public int Assets { get; set;  }
    }
}
