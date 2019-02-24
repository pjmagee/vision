using System;

namespace Vision.Shared
{
    public class CiCdDto
    {
        public Guid CiCdId { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public CiCdKind Kind { get; set; }
    }
}
