using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public class AggregateVcsChecker : IVcsChecker
    {
        private readonly IEnumerable<IVcsChecker> gitServices;

        public AggregateVcsChecker(BitBuckerChecker bitBucketService, GitlabChecker gitlabService)
        {
            this.gitServices = new IVcsChecker[] { bitBucketService, gitlabService };
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository)
        {
            List<Asset> results = new List<Asset>();

            foreach (var gitService in gitServices)
            {
                IEnumerable<Asset> assets = await gitService.GetAssetsAsync(repository);
                results.AddRange(assets);
            }

            return results;
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source)
        {
            List<Repository> results = new List<Repository>();

            foreach (var gitService in gitServices)
            {
                IEnumerable<Repository> repositories = await gitService.GetRepositoriesAsync(source);
                results.AddRange(repositories);
            }

            return results;
        }
    }
}


