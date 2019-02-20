using System.Collections.Generic;

namespace Vision.Core
{
    public class Squad : Entity
    {
        public string Name { get; set; }
        public virtual ICollection<GitRepository> Repositories { get; set; }
    }
}
