using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Vision.Shared;

namespace Vision.Core
{
    public class NuGetVersionService : IVersionChecker
    {
        private readonly VisionDbContext context;
        private static readonly HttpClient client = new HttpClient();

        public DependencyKind Kind => DependencyKind.NuGet;

        public NuGetVersionService(VisionDbContext context) => this.context = context;

        public async Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            foreach (Registry registry in await context.Registries.Where(registry => registry.Kind == DependencyKind.NuGet).ToListAsync())
            {                
                try
                {
                    client.BaseAddress = new Uri(registry.Endpoint);

                    if (registry.Endpoint.EndsWith("index.json"))
                    {
                        // root API v3
                        JObject root = JObject.Parse(await client.GetStringAsync(string.Empty));

                        // RegistrationsBaseUrl Resource
                        string endppoint = root["resources"].Single(x => x["@type"].Value<string>() == "RegistrationsBaseUrl")["@id"].Value<string>();
                        client.BaseAddress = new Uri(endppoint);

                        // Dependency Registration Response
                        var dependencyResponse = await client.GetStringAsync(new Uri($"{dependency}/index.json", UriKind.Relative));

                        var response = JObject.Parse(dependencyResponse);
                        var latest = response["items"].First["upper"].Value<string>();
                        return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = latest, IsLatest = true };
                    }
                    else
                    {
                        // v2 XML API (nexus, etc)
                    }
                }
                catch (Exception)
                {
                    // try next registry
                }
            }

            throw new Exception($"Could not find latest version for {dependency.Name}");
        }      
    }
}
