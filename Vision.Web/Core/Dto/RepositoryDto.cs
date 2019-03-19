namespace Vision.Web.Core
{
    using System;

    public class RepositoryDto
    {
        private string webUrl;

        public Guid VersionControlId { get; set; }
        public Guid RepositoryId { get; set; }
        public string Url { get; set; }
        public string WebUrl { get => new Uri(webUrl).GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped); set => webUrl = value; }
        public int Assets { get; set; }

        public bool IsIgnored { get; set; }
    }
}
