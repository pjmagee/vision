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
    public class NuGetVersionProvider : AbstractVersionProvider
    {        
        private static readonly HttpClient client = new HttpClient();               

        public NuGetVersionProvider(VisionDbContext context, IDataProtectionProvider provider, ILogger<NuGetVersionProvider> logger) : base(context, provider, logger)
        {
            
        } 

        public override bool Supports(DependencyKind kind) => kind == DependencyKind.NuGet;

        protected override async Task<DependencyVersion> GetLatestMetaDataAsync(Registry registry, Dependency dependency)
        {
            HttpResponseMessage response = await client.GetAsync(registry.Endpoint);

            string mediaType = response.Content.Headers.ContentType.MediaType;

            if (IsV2APIEndpoint(mediaType))
            {
                return await HandleV2API(registry, dependency);
            }
            else if (IsV3APIEndpoint(mediaType))
            {
                return await HandleV3API(registry, dependency);
            }
            else
            {
                throw new Exception($"Unsupported NuGet registry: {registry.Endpoint}");
            }

            throw new Exception("Could not find dependency");
        }

        private async Task<DependencyVersion> HandleV2API(Registry registry, Dependency dependency)
        {
            try
            {
                // THIS ONE WORKS ON OFFICAL NUGET V2 XML API, BUT NOT ON NEXUS!!!!
                // var root = XDocument.Parse(await client.GetStringAsync($"{registry.Endpoint}/FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion eq true"));

                Uri endpoint = new Uri(new Uri(registry.Endpoint.Trim('/') + "/", UriKind.Absolute), new Uri($"FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion", UriKind.Relative));

                logger.LogInformation($"Checking {endpoint} for latest version");

                XDocument root = XDocument.Parse(await client.GetStringAsync(endpoint));
                string latest = root.XPathSelectElement("(//*[local-name() = '" + "Version" + "'])").Value;

                logger.LogInformation($"Found latest version for {dependency.Name}: {latest}");

                return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Could not get version for {dependency.Name}");
                throw;
            }
        }

        private async Task<DependencyVersion> HandleV3API(Registry registry, Dependency dependency)
        {
            try
            {
                JObject root = JObject.Parse(await client.GetStringAsync(registry.Endpoint));
                string registration = root["resources"].Single(x => x["@type"].Value<string>() == "RegistrationsBaseUrl")["@id"].Value<string>();

                Uri endpoint = new Uri(
                    new Uri(registration.Trim('/') + "/", UriKind.Absolute),
                    new Uri(dependency.Name.ToLower() + "/" + "index.json", UriKind.Relative));

                logger.LogInformation($"Checking {endpoint} for latest version");

                string registryReponse = await client.GetStringAsync(endpoint);
                JObject json = JObject.Parse(registryReponse);
                string latest = json["items"].First["upper"].Value<string>();

                logger.LogInformation($"Found latest version for {dependency.Name}: {latest}");

                return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Could not get version for {dependency.Name}");
                throw;
            }
        }

        private static bool IsV2APIEndpoint(string mediaType) => string.Equals(mediaType, "application/xml", StringComparison.CurrentCultureIgnoreCase);

        private static bool IsV3APIEndpoint(string mediaType) => string.Equals(mediaType, "application/json", StringComparison.CurrentCultureIgnoreCase);
    }
}
