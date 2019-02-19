using Vision.Shared;

namespace Vision.Core
{
    public class BuildSources : Entity
    {       
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public BuildKind Kind { get; set; }
    }
}
