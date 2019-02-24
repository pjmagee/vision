using System;

namespace Vision.Shared
{
    public class CiCdBuildDto
    {
        public Guid CiCdId { get; set; }
        public string Name { get; set; }
        public string WebUrl { get; set; }
        public CiCdKind Kind { get; set; }
    }
}
