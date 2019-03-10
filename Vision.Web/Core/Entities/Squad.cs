using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Squad : Entity
    {
        public string Name { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; }
    }
}
