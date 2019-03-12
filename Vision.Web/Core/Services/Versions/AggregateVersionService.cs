using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class AggregateVersionService : IVersionService
    {
        private readonly IEnumerable<IVersionService> versionServices;
        private readonly ILogger<AggregateVersionService> logger;

        public AggregateVersionService(
            NuGetVersionService nuGetVersionService,
            DockerVersionService dockerVersionService,
            NpmVersionService nPMVersionService,
            ILogger<AggregateVersionService> logger)
        {
            versionServices = new IVersionService[] { nuGetVersionService, dockerVersionService, nPMVersionService };
            this.logger = logger;
        }

        public DependencyKind Kind => throw new NotSupportedException("This is an aggregated version service.");

        public async Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            foreach(IVersionService service in versionServices.Where(service => service.Supports(dependency.Kind)))
            {
                try
                {
                    var latest = await service.GetLatestVersionAsync(dependency);

                    if(latest != null)
                    {
                        logger.LogInformation($"Aggregate version service found latest version for {dependency.Name}: {latest}");
                        return latest;
                    }
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"Aggregate version service failed to retrieve version using {service.GetType().Name}");
                }
            }

            return new DependencyVersion { IsLatest = false, Version = "UNKNOWN", Dependency = dependency, DependencyId = dependency.Id };
        }

        public bool Supports(DependencyKind kind) => versionServices.Any(s => s.Supports(kind));
    }    
}
