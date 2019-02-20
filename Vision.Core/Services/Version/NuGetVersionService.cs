﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Vision.Shared;

namespace Vision.Core
{
    public class NuGetVersionService : IVersionService
    {
        private readonly VisionDbContext context;
        private static readonly HttpClient client = new HttpClient();

        public DependencyKind Kind => DependencyKind.NuGet;

        public NuGetVersionService(VisionDbContext registryService)
        {
            this.context = registryService;
        }

        public async Task<DependencyVersion> GetLatestVersion(Dependency dependency)
        {
            foreach (Registry registry in await context.Registries.Where(x => x.Kind == DependencyKind.NuGet).ToListAsync())
            {                
                try
                {
                    client.DefaultRequestHeaders.Add("API-KEY", registry.ApiKey);
                    client.BaseAddress = new Uri(registry.Endpoint);
                    var statusResponse = await client.GetAsync(string.Empty);
                    var contentType = statusResponse.Headers.GetValues("Content-Type");

                    if (contentType.Contains("application/xml"))
                    {
                        // NuGet API V2 (XML)
                    }
                    else if (contentType.Contains("application/json"))
                    {
                        // NuGet API V3 index.json (JSON)
                        var json = await client.GetStringAsync(new Uri($"registration3/{dependency}/index.json", UriKind.Relative));
                        var response = JObject.Parse(json);
                        var latest = response["items"].First["upper"].Value<string>();
                        return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Id = Guid.NewGuid(), IsVulnerable = false, Version = latest, VulnerabilityUrl = null };
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
