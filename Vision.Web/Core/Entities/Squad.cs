using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Squad : Entity
    {
        public string Name { get; set; }
        public virtual ICollection<VcsRepository> Repositories { get; set; }
    }
}
