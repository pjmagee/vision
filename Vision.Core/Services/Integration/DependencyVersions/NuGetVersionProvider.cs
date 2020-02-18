using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Core
{

    public class NuGetVersionProvider : IDependencyVersionProvider
    {        
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<NuGetVersionProvider> logger;

        public NuGetVersionProvider(ILogger<NuGetVersionProvider> logger)
        {
            this.logger = logger;
        } 

        public bool Supports(EcosystemKind kind) => kind == EcosystemKind.NuGet;

        public async Task<DependencyVersion> GetLatestMetaDataAsync(RegistryDto registry, Dependency dependency)
        {           
            HttpResponseMessage response = await client.GetAsync(registry.Endpoint);
            string mediaType = response.Content.Headers.ContentType.MediaType;

            if (IsV2APIEndpoint(mediaType))
            {
                return await HandleV2API(registry, dependency);
            }

            if (IsV3APIEndpoint(mediaType))
            {
                return await HandleV3API(registry, dependency);
            }

            throw new Exception("Unhandled MediaType");            
        }

        private async Task<DependencyVersion> HandleV2API(RegistryDto registry, Dependency dependency)
        {
            Uri endpoint = new Uri(new Uri(registry.Endpoint.Trim('/') + "/", UriKind.Absolute), new Uri($"FindPackagesById()?id='{dependency.Name}'&$filter=IsLatestVersion", UriKind.Relative));
            XDocument root = XDocument.Parse(await client.GetStringAsync(endpoint));
            string version = root.XPathSelectElement("(//*[local-name() = '" + "Version" + "'])").Value;
            string projectUrl = root.XPathSelectElement("(//*[local-name() = '" + "ProjectUrl" + "'])")?.Value;

            return new DependencyVersion { IsLatest = true, Version = version, ProjectUrl = projectUrl, Dependency = dependency, DependencyId = dependency.Id, };
        }

        private async Task<DependencyVersion> HandleV3API(RegistryDto registry, Dependency dependency)
        {
            JObject root = JObject.Parse(await client.GetStringAsync(registry.Endpoint));
            string registration = root["resources"].Single(x => x["@type"].Value<string>() == "RegistrationsBaseUrl")["@id"].Value<string>();
            Uri endpoint = new Uri(new Uri(registration.Trim('/') + "/", UriKind.Absolute), new Uri(dependency.Name.ToLower() + "/" + "index.json", UriKind.Relative));

            logger.LogTrace($"Checking {endpoint} for latest version");

            string registryReponse = await client.GetStringAsync(endpoint);
            JObject json = JObject.Parse(registryReponse);
            string version = json["items"].First["upper"].Value<string>();
            string projectUrl = null; // TODO

            logger.LogTrace($"Found latest version for {dependency.Name}: {version}");

            return new DependencyVersion { IsLatest = true, Version = version, ProjectUrl = projectUrl, Dependency = dependency, DependencyId = dependency.Id, };
        }

        private static bool IsV2APIEndpoint(string mediaType) => string.Equals(mediaType, "application/xml", StringComparison.CurrentCultureIgnoreCase);

        private static bool IsV3APIEndpoint(string mediaType) => string.Equals(mediaType, "application/json", StringComparison.CurrentCultureIgnoreCase);
    }
}
