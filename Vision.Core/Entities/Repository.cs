using System;
using System.Collections.Generic;

namespace Vision.Core
{
    public class GitRepository : Entity
    {
        public string GitUrl { get; set; }
        public string WebUrl { get; set; }
        public GitSource Source { get; set; }
        public Guid SourceId { get; set; }
        public List<Asset> Assets { get; set; }
    }   
}
