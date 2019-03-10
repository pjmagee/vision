using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class VersionControl : Entity
    {
        public VersionControlKind Kind { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public bool IsEnabled { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; }
    }
}
