using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Vision.Web.Core
{
    public class NPMVersionService : VersionService
    {
        private static readonly HttpClient client = new HttpClient();

        public NPMVersionService(VisionDbContext context, IDataProtectionProvider provider, ILogger<VersionService> logger) : base(context, provider, logger)
        {

        }

        public override DependencyKind Kind => DependencyKind.Npm;

        protected override async Task<DependencyVersion> NextAsync(Registry registry, Dependency dependency)
        {
            var response = await client.GetStringAsync($"{registry.Endpoint}/{dependency}");
            var resp = JObject.Parse(response);
            var latest = resp["dist-tags"]["latest"].ToString();
            return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = latest };
        }
    }
}
