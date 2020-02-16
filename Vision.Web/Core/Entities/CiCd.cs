using System.Collections.Generic;

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

        public static IEnumerable<CiCd> MockData()
        {
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.TeamCity, Endpoint = "http://teamcity.xpa.rbxd.ds:8080" };
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.TeamCity, Endpoint = "http://dataservices.teamcity.xpa.rbxd.ds:8080" };
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.TeamCity, Endpoint = "http://xpaqhsctrd100v.xpa.rbxd.ds:8080" };
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.Jenkins, Endpoint = "https://jenkins.xpa.rbxd.ds/", ApiKey = "11c7147cf44cf7072cad8305ba26af6139" };
        }
    }
}

