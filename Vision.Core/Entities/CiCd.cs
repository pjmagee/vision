using Vision.Shared;

namespace Vision.Core
{
    public class CiCd : Entity
    {       
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public CiCdKind Kind { get; set; }
    }
}
