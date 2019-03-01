using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class Repository : Entity
    {
        public string Url { get; set; }
        public string WebUrl { get; set; }
        public virtual VersionControl VersionControl { get; set; }
        public Guid VersionControlId { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
    }   
}
