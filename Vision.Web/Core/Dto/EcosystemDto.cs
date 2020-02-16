using System;

namespace Vision.Web.Core
{
    public class EcosystemDto
    {
        public string Name { get; set; }
        public int Versions { get; set; }
        public Guid EcosystemId { get; set; }
        public int Assets { get; set; }
    }
}
