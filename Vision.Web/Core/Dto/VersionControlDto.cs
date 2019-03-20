namespace Vision.Web.Core
{
    using System;

    public class VersionControlDto
    {
        public Guid VersionControlId { get; set; }
        public VersionControlKind Kind { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public int Repositories { get; set; }
        public bool IsEnabled { get; set; }
    }
}
