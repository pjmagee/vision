using System;

namespace Vision.Web.Core
{
    public class RuntimeVersion : Entity
    {
        public string Version { get; set; }
        public Guid RuntimeId { get; set; }
        public virtual Runtime Runtime { get; set; }
    }
}
