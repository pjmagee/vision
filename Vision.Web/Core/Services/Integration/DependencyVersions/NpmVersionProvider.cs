using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{
    public class NpmVersionProvider : IDependencyVersionProvider
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<NpmVersionProvider> logger;

        public NpmVersionProvider(ILogger<NpmVersionProvider> logger)
        {
            this.logger = logger;
        }

        public bool Supports(EcosystemKind kind) => kind == EcosystemKind.Npm;

        public async Task<DependencyVersion> GetLatestMetaDataAsync(RegistryDto registry, Dependency dependency)
        {
            string query = new Uri(new Uri(registry.Endpoint.Trim('/') + "/", UriKind.Absolute), new Uri($"{dependency.Name}", UriKind.Relative)).ToString();
            string json = await client.GetStringAsync(query);
            JObject resp = JObject.Parse(json);
            string version = resp["dist-tags"]["latest"].ToString();

            return new DependencyVersion { IsLatest = true, Version = version, ProjectUrl = "", Dependency = dependency, DependencyId = dependency.Id, };
        }
    }
}
