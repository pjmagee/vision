using System.ComponentModel.DataAnnotations.Schema;

namespace Vision.Web.Core
{
    public class CiCd : Entity
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsGuestEnabled { get; set; }
        public CiCdKind Kind { get; set; }
    }
}

