using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class GitRepository : Entity
    {
        public string GitUrl { get; set; }
        public string WebUrl { get; set; }
        public virtual GitSource GitSource { get; set; }
        public Guid GitSourceId { get; set; }
        public virtual IList<Asset> Assets { get; set; }
    }   
}
