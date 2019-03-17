using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public abstract class AbstractVersionProvider : IVersionProvider
    {
        protected readonly VisionDbContext context;
        protected readonly IDataProtector protector;
        protected readonly ILogger<AbstractVersionProvider> logger;

        public AbstractVersionProvider(VisionDbContext context, IDataProtectionProvider provider, ILogger<AbstractVersionProvider> logger)
        {
            this.context = context;
            this.protector = provider.CreateProtector("Registry");
            this.logger = logger;
        }

        public async Task<DependencyVersion> GetLatestMetaDataAsync(Dependency dependency)
        {
            List<Registry> registries = await context.Registries
                .Where(registry => registry.Kind == dependency.Kind && registry.IsEnabled)
                .OrderBy(registry => registry.IsPublic)
                .ToListAsync();

            foreach (Registry registry in registries)
            {
                try
                {
                    logger.LogInformation($"Searching latest version for {dependency.Name} at registry: {registry.Endpoint}.");

                    DependencyVersion result = await GetLatestMetaDataAsync(registry, dependency);

                    if (result == null)
                    {
                        throw new InvalidOperationException($"DependencyVersion returned for {dependency.Name} is null.");
                    }

                    logger.LogInformation($"Found latest version {result.Version} for {dependency.Name}");

                    return result;
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"Error finding latest version for {dependency.Name} at registry: {registry.Endpoint}.");
                }
            }

            logger.LogWarning($"Did not find latest version for {dependency.Name} in any registry. Returning version 'UNKNOWN' and Latest = false.");

            return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, Version = "UNKNOWN", IsLatest = false };
        }

        protected abstract Task<DependencyVersion> GetLatestMetaDataAsync(Registry registry, Dependency dependency);

        public abstract bool Supports(DependencyKind kind);
    }
}
