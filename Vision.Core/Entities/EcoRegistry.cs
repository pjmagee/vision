using System.Collections.Generic;

namespace Vision.Core
{
    public class EcoRegistry : Entity
    {
        public string Endpoint { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsPublic { get; set; }
        public EcosystemKind Kind { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static IEnumerable<EcoRegistry> MockData()
        {
            yield return new EcoRegistry { IsPublic = false, IsEnabled = true, Kind = EcosystemKind.NuGet,  Endpoint = "https://nexus3.company.com:8080/repository/nuget-group/" };
            yield return new EcoRegistry { IsPublic = false, IsEnabled = true, Kind = EcosystemKind.Npm,    Endpoint = "https://nexus3.company.com:8080/repository/npm-group/" };
            yield return new EcoRegistry { IsPublic = false, IsEnabled = true, Kind = EcosystemKind.Docker, Endpoint = "nexus3.company.comp:8084" };
            yield return new EcoRegistry { IsPublic = true, IsEnabled = true, Kind = EcosystemKind.NuGet,   Endpoint = "https://api.nuget.org/v3/index.json" };
        }
    }
}
