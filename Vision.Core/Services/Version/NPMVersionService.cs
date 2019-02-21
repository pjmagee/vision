using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Vision.Shared;

namespace Vision.Core
{
    public class NPMVersionService : IVersionService
    {
        private readonly VisionDbContext context;
        private static readonly HttpClient client = new HttpClient();

        public DependencyKind Kind => DependencyKind.Npm;

        public NPMVersionService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            foreach (Registry registry in await context.Registries.Where(x => x.Kind == DependencyKind.Npm).ToListAsync())
            {
                try
                {
                    client.BaseAddress = new Uri(registry.Endpoint);
                    var response = await client.GetStringAsync($"/{dependency}");
                    var resp = JObject.Parse(response);
                    var latest = resp["dist-tags"]["latest"].ToString();
                    return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Id = Guid.NewGuid(), Version = latest };
                }
                catch(Exception)
                {
                    // try next registry
                }
            }

            throw new Exception($"Could not find latest version for {dependency.Name}");
        }
    }
}
