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
               

        public NuGetVersionService(VisionDbContext context, IDataProtectionProvider provider, ILogger<NuGetVersionService> logger) : base(context, provider, logger)
        {
            
        }

        public override bool Supports(DependencyKind kind) => kind == DependencyKind.NuGet;

        protected override async Task<DependencyVersion> GetLatestVersionAsync(Registry registry, Dependency dependency)
        {
            // TODO: Wait until Nexus3 actually implements a v3 NuGet API
            // https://issues.sonatype.org/browse/NEXUS-10886
            if (registry.Endpoint.EndsWith("index.json"))
            {
                // root API v3
                JObject root = JObject.Parse(await client.GetStringAsync(registry.Endpoint));

                // RegistrationsBaseUrl Resource
                string registration = root["resources"].Single(x => x["@type"].Value<string>() == "RegistrationsBaseUrl")["@id"].Value<string>();

                // Dependency Registration Response
                var endpoint = $"{registration.TrimEnd('/')}/{dependency.Name.ToLower()}/index.json";

                logger.LogInformation($"Checking {endpoint} for latest version"); 

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
                    string endpoint = new Uri(new Uri(registry.Endpoint), new Uri($"/FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion", UriKind.Relative)).ToString();

                    logger.LogInformation($"Checking {endpoint} for latest version");

                    var root = XDocument.Parse(await client.GetStringAsync(endpoint));
                    var latest = root.XPathSelectElement("(//*[local-name() = '" + "Version" + "'])").Value;

                    if(!string.IsNullOrWhiteSpace(latest))
                    {
                        logger.LogInformation($"Found latest version for {dependency.Name}: {latest}");
                        return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
                    }                    
                }
                catch(NullReferenceException e)
                {
                    // poor XML handling :-)
                    logger.LogError(e, $"Error extracting information for: {dependency.Name}");
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
