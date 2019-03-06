using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Vision.Shared;

namespace Vision.Core
{

    public class NuGetVersionService : IVersionChecker
    {
        private readonly VisionDbContext context;
        private static readonly HttpClient client = new HttpClient();

        public DependencyKind Kind => DependencyKind.NuGet;

        public NuGetVersionService(VisionDbContext context) => this.context = context;

        public async Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            foreach (Registry registry in await context.Registries.Where(registry => registry.Kind == DependencyKind.NuGet).ToListAsync())
            {                
                try
                {
                    if (registry.Endpoint.EndsWith("index.json"))
                    {
                        // root API v3
                        JObject root = JObject.Parse(await client.GetStringAsync(registry.Endpoint));

                        // RegistrationsBaseUrl Resource
                        string registration = root["resources"].Single(x => x["@type"].Value<string>() == "RegistrationsBaseUrl")["@id"].Value<string>().TrimEnd('/');

                        // Dependency Registration Response
                        var endpoint = $"{registration}/{dependency.Name.ToLower()}/index.json";

                        var dependencyResponse = await client.GetStringAsync(endpoint);

                        var response = JObject.Parse(dependencyResponse);
                        var latest = response["items"].First["upper"].Value<string>();
                        return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = latest, IsLatest = true };
                    }
                    else
                    {
                        var root = XDocument.Parse(await client.GetStringAsync($"{registry.Endpoint}/FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion eq true"));
                        var latest = root.XPathSelectElement("(//*[local-name() = '" + "Version" + "'])").Value;
                        return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = latest, IsLatest = true };
                    }
                }
                catch (Exception e)
                {
                    // try next registry
                }
            }

            return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = "UNKNOWN", IsLatest = false };
        }      
    }
}
