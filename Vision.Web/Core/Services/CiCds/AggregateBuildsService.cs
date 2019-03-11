using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class AggregateBuildsService : ICICDBuildsService
    {
        private readonly VisionDbContext context;
        private readonly ILogger<AggregateBuildsService> logger;

        private readonly IEnumerable<ICICDBuildsService> buildServices;

        public AggregateBuildsService(
            VisionDbContext context,
            ILogger<AggregateBuildsService> logger,
            JenkinsBuildsService jenkinsBuildsService,
            TeamCityBuildsService teamCityBuildsService)
        {
            this.context = context;
            this.logger = logger;
            this.buildServices = new ICICDBuildsService[] { jenkinsBuildsService, teamCityBuildsService };
        }

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = context.Repositories.Find(repositoryId);

            List<CiCdBuildDto> builds = new List<CiCdBuildDto>();

            foreach (var service in buildServices)
            {
                var results = await service.GetBuildsByRepositoryIdAsync(repositoryId);
                builds.AddRange(results);
            }
            
            logger.LogInformation($"Found {builds.Count} for {repository.WebUrl}");

            return builds;
        }

        public bool Supports(CiCdKind Kind) => true;
    }
}
