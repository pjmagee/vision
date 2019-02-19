using System.Collections.Generic;
using Vision.Shared;

namespace Vision.Core
{
    public class GitSource : Entity
    {
        public GitKind Kind { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public List<GitRepository> Repositories { get; set; }
    }
}
