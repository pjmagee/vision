using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Vision.Shared;

namespace Vision.Core
{
    public class Registry : Entity
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public bool IsPublic { get; set; }
        public DependencyKind Kind { get; set; }
        public bool IsEnabled { get; set; }
        public virtual IList<Dependency> Dependencies { get; set; }
    }
}
