using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{
    public class NpmVersionProvider : AbstractVersionProvider
    {
        private static readonly HttpClient client = new HttpClient();
        
        public NpmVersionProvider(VisionDbContext context, IDataProtectionProvider provider, ILogger<NpmVersionProvider> logger) : base(context, provider, logger)
        {

        }

        public override bool Supports(DependencyKind kind) => kind == DependencyKind.Npm;

        protected override async Task<DependencyVersion> GetLatestMetaDataAsync(Registry registry, Dependency dependency)
        {
            string query = new Uri(new Uri(registry.Endpoint.Trim('/') + "/", UriKind.Absolute), new Uri($"{dependency.Name}", UriKind.Relative)).ToString();
            string json = await client.GetStringAsync(query);
            JObject resp = JObject.Parse(json);
            string latest = resp["dist-tags"]["latest"].ToString();
            return new DependencyVersion { Version = latest, IsLatest = true, Dependency = dependency, DependencyId = dependency.Id };
        }
    }
}
