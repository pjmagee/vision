using System.Collections.Generic;
using Vision.Shared;

namespace Vision.Core
{
    public class VersionControl : Entity
    {
        public VersionControlKind Kind { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; }
    }
}
