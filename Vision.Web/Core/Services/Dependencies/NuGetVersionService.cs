using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{

    public class NuGetVersionService : VersionService
    {        
        private static readonly HttpClient client = new HttpClient();
        
        public override DependencyKind Kind => DependencyKind.NuGet;

        public NuGetVersionService(VisionDbContext context, IDataProtectionProvider provider, ILogger<NuGetVersionService> logger) : base(context, provider, logger)
        {
            
        }

        protected override async Task<DependencyVersion> NextAsync(Registry registry, Dependency dependency)
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

            throw new Exception("Could not find dependency");
        }
    }
}
