using System;

namespace Vision.Web.Core
{
    public class RuntimeDto
    {
        public string Name { get; set; }
        public int Versions { get; set; }
        public Guid RuntimeId { get; set; }
        public int Assets { get; set; }
    }
}
