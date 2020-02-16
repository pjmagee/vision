using Microsoft.AspNetCore.DataProtection;
using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class ArtifactRegistry : Entity
    {
        public string Endpoint { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsPublic { get; set; }
        public DependencyKind Kind { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static IEnumerable<ArtifactRegistry> MockData()
        {
            yield return new ArtifactRegistry { IsPublic = false, IsEnabled = true, Kind = DependencyKind.NuGet, Endpoint = "https://nexus3.xpa.rbxd.ds/repository/nuget-group/" };
            yield return new ArtifactRegistry { IsPublic = false, IsEnabled = true, Kind = DependencyKind.Npm, Endpoint = "https://nexus3.xpa.rbxd.ds/repository/npm-group/" };
            yield return new ArtifactRegistry { IsPublic = false, IsEnabled = true, Kind = DependencyKind.Docker, Endpoint = "nexus3.xpa.rbxd.ds:8080" };
            yield return new ArtifactRegistry { IsPublic = true, IsEnabled = true, Kind = DependencyKind.NuGet, Endpoint = "https://api.nuget.org/v3/index.json" };
        }
    }
}
