using System;

namespace Vision.Shared.Api
{
    public class GitSourceDto
    {
        public Guid SourceId { get; set; }
        public GitKind Kind { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public int Repositories { get; set; }
    }
}
