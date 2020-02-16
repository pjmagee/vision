using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Runtime : Entity
    {
        public string Name { get; set; } // Node, .NET, Java, Ruby, Python
        public virtual ICollection<RuntimeVersion> Versions { get; set; }
    }
}
