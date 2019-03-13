using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{
    public class NpmVersionService : VersionService
    {
        private static readonly HttpClient client = new HttpClient();
        
        public NpmVersionService(VisionDbContext context, IDataProtectionProvider provider, ILogger<NpmVersionService> logger) : base(context, provider, logger)
        {

        }

        public override bool Supports(DependencyKind kind) => kind == DependencyKind.Npm;

        protected override async Task<DependencyVersion> GetLatestVersionAsync(Registry registry, Dependency dependency)
        {
            string query = new Uri(new Uri(registry.Endpoint), new Uri($"/{dependency.Name}", UriKind.Relative)).ToString();

            logger.LogInformation($"Checking {query} for latest version");

            string json = await client.GetStringAsync(query);
            JObject resp = JObject.Parse(json);
            string latest = resp["dist-tags"]["latest"].ToString();

            return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
        }
    }
}
