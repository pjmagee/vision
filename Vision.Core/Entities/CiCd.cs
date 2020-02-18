using System.Collections.Generic;

namespace Vision.Core
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
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.TeamCity, Endpoint = "http://teamcity1.company.com:8080" };
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.TeamCity, Endpoint = "http://teamcity2.company.com:8080" };
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.TeamCity, Endpoint = "http://teamcity3.company.com:8080" };
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.Jenkins, Endpoint =  "http://jenkins1.company.com/", ApiKey = "" };
            yield return new CiCd { IsEnabled = false, Kind = CiCdKind.Jenkins, Endpoint =  "http://jenkins2.company.com/", ApiKey = "" };
        }
    }
}

