using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public class AggregateVcsChecker : IVcsChecker
    {
        private readonly IEnumerable<IVcsChecker> vcCheckers;

        public AggregateVcsChecker(BitBucketChecker bitBucketService, GitlabChecker gitlabService)
        {
            vcCheckers = new IVcsChecker[] { bitBucketService, gitlabService };
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository)
        {
            List<Asset> results = new List<Asset>();

            foreach (IVcsChecker vcsChecker in vcCheckers)
            {
                IEnumerable<Asset> assets = await vcsChecker.GetAssetsAsync(repository);
                results.AddRange(assets);
            }

            return results;
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source)
        {
            List<Repository> results = new List<Repository>();

            foreach (IVcsChecker vcsChecker in vcCheckers)
            {
                IEnumerable<Repository> repositories = await vcsChecker.GetRepositoriesAsync(source);
                results.AddRange(repositories);
            }

            return results;
        }
    }
}


