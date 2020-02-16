using System;

namespace Vision.Web.Core
{
    public class EcosystemVersionDto
    {
        public string Version { get; set; }
        public Guid EcosystemVersionId { get; set; }
        public Guid EcosystemId { get; set; }
        public int Assets { get; set; }
    }
}
