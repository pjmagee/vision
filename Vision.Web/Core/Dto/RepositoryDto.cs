namespace Vision.Web.Core
{
    using System;

    public class RepositoryDto
    {
        public Guid VersionControlId { get; set; }
        public Guid RepositoryId { get; set; }
        public string Url { get; set; }
        public string WebUrl { get; set; }
        public int Assets { get; set;  }

        public bool IsIgnored { get; set; }
    }
}
