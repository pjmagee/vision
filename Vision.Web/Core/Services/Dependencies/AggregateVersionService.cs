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
            NPMVersionService nPMVersionService,
            ILogger<AggregateVersionService> logger)
        {
            versionServices = new IVersionService[] { nuGetVersionService, dockerVersionService, nPMVersionService };
            this.logger = logger;
        }

        public DependencyKind Kind => throw new NotSupportedException("This is an aggregated version service.");

        public async Task<DependencyVersion> GetLatestVersionAsync(Dependency dependency)
        {
            foreach(IVersionService service in versionServices.Where(s => s.Kind == dependency.Kind))
            {
                try
                {
                    return await service.GetLatestVersionAsync(dependency);
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"Aggregate version service failed to retrieve version using {service.ToString()}");
                }
            }

            return new DependencyVersion { Dependency = dependency, DependencyId = dependency.Id, IsLatest = false, Version = "UNKNOWN" };
        }
    }
}
