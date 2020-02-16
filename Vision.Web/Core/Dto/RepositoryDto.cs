namespace Vision.Web.Core
{
    using System;

    public class RepositoryDto
    {
        public string WebUrl { get; set; }

        public Guid VersionControlId { get; set; }

        public Guid RepositoryId { get; set; }

        public string Url { get; set; }

        public string PathAndQuery => new Uri(WebUrl).GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped);

        public string AbsoluteUri => new Uri(WebUrl).GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped);

        public int Assets { get; set; }

        public bool IsIgnored { get; set; }
    }
}
