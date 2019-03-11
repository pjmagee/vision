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

                logger.LogInformation($"Found latest version for {dependency.Name}: {latest}");

                return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
            }
            else
            {
                // THIS ONE WORKS ON OFFICAL NUGET V2 XML API, BUT NOT ON NEXUS!!!!
                // var root = XDocument.Parse(await client.GetStringAsync($"{registry.Endpoint}/FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion eq true"));

                try
                {
                    var root = XDocument.Parse(await client.GetStringAsync($"{registry.Endpoint}/FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion"));
                    var latest = root.XPathSelectElement("(//*[local-name() = '" + "Version" + "'])").Value;

                    if(!string.IsNullOrWhiteSpace(latest))
                    {
                        logger.LogInformation($"Found latest version for {dependency.Name}: {latest}");
                        return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
                    }                    
                }
                catch(NullReferenceException)
                {
                    // poor XML handling :-)
                }
                catch(HttpRequestException e)
                {
                    logger.LogError(e, $"HTTP Error when searching version for: {dependency.Name}");
                }                
            }

            throw new Exception("Could not find dependency");
        }
    }
}
