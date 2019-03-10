using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class AggregateVersionControlService : IVersionControlService
    {
        private readonly IEnumerable<IVersionControlService> providers;

        public AggregateVersionControlService(BitBucketProvider bitBucketService, GitlabProvider gitlabService)
        {
            providers = new IVersionControlService[] { bitBucketService, gitlabService };
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository)
        {
            List<Asset> results = new List<Asset>();

            foreach (IVersionControlService provider in providers)
            {
                IEnumerable<Asset> assets = await provider.GetAssetsAsync(repository);
                results.AddRange(assets);
            }

            return results;
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source)
        {
            List<Repository> results = new List<Repository>();

            foreach (IVersionControlService provider in providers)
            {
                IEnumerable<Repository> repositories = await provider.GetRepositoriesAsync(source);
                results.AddRange(repositories);
            }

            return results;
        }
    }
}


