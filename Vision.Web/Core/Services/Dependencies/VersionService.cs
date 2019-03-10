using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public abstract class VersionService : IVersionService
    {
        protected readonly VisionDbContext context;
        protected readonly IDataProtector protector;
        protected readonly ILogger<VersionService> logger;

        public abstract DependencyKind Kind { get; }

        public VersionService(VisionDbContext context, IDataProtectionProvider provider, ILogger<VersionService> logger)
        {
            this.context = context;
            this.protector = provider.CreateProtector("Registry");
            this.logger = logger;
        }

        public async Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            List<Registry> registries = await context.Registries
                .Where(registry => registry.Kind == Kind && registry.IsEnabled)
                .OrderBy(registry => registry.IsPublic)
                .ToListAsync();

            foreach (Registry registry in registries)
            {
                try
                {
                    DependencyVersion result = await NextAsync(registry, dependency);
                    if (result != null) return result;
                }
                catch(Exception e)
                {
                    logger.LogInformation(e, $"Could not find {dependency.Name} in {registry.Endpoint}");
                }
            }

            return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = "UNKNOWN", IsLatest = false };
        }

        protected abstract Task<DependencyVersion> NextAsync(Registry registry, Dependency dependency);
    }
}
