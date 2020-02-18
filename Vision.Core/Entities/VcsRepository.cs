using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class VcsRepository : Entity
    {
        public string Url { get; set; }
        public string WebUrl { get; set; }
        public Guid VcsId { get; set; }
        public virtual Vcs Vcs { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public bool IsIgnored { get; set; } = false;
    }
}
