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

        protected override async Task<DependencyVersion> GetLatestMetaDataAsync(Registry registry, Dependency dependency)
        {
            HttpResponseMessage response = await client.GetAsync(registry.Endpoint);

            var contentTypes = response.Headers.GetValues("Content-Type"); 
            
            if (contentTypes.Contains("application/json"))
            {
                JObject root = JObject.Parse(await client.GetStringAsync(registry.Endpoint));
                string registration = root["resources"].Single(x => x["@type"].Value<string>() == "RegistrationsBaseUrl")["@id"].Value<string>();
                string endpoint = $"{registration.TrimEnd('/')}/{dependency.Name.ToLower()}/index.json";

                logger.LogInformation($"Checking {endpoint} for latest version");

                string registryReponse = await client.GetStringAsync(endpoint);
                JObject json = JObject.Parse(registryReponse);
                string latest = json["items"].First["upper"].Value<string>();

                logger.LogInformation($"Found latest version for {dependency.Name}: {latest}");

                return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
            }
            else if(contentTypes.Contains("application/xml"))
            {
                // THIS ONE WORKS ON OFFICAL NUGET V2 XML API, BUT NOT ON NEXUS!!!!
                // var root = XDocument.Parse(await client.GetStringAsync($"{registry.Endpoint}/FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion eq true"));

                try
                {
                    string endpoint = new Uri(new Uri(registry.Endpoint), new Uri($"/FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion", UriKind.Relative)).ToString();

                    logger.LogInformation($"Checking {endpoint} for latest version");

                    XDocument root = XDocument.Parse(await client.GetStringAsync(endpoint));
                    string latest = root.XPathSelectElement("(//*[local-name() = '" + "Version" + "'])").Value;

                    if(!string.IsNullOrWhiteSpace(latest))
                    {
                        logger.LogInformation($"Found latest version for {dependency.Name}: {latest}");
                        return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
                    }
                }
                catch(NullReferenceException e)
                {
                    logger.LogError(e, $"Error extracting information for: {dependency.Name}");
                }
                catch(HttpRequestException e)
                {
                    logger.LogError(e, $"HTTP Error when searching version for: {dependency.Name}");
                }                
            }
            else
            {
                throw new Exception($"Unsupported NuGet registry: {registry.Endpoint}");
            }

            throw new Exception("Could not find dependency");
        }
    }
}
