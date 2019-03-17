using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class AggregateVersionControlProvider : IVersionControlProvider
    {
        private readonly IEnumerable<IVersionControlProvider> providers;
        private readonly ILogger<AggregateVersionControlProvider> logger;

        public AggregateVersionControlProvider(BitBucketProvider bitBucketService, GitlabProvider gitlabService, ILogger<AggregateVersionControlProvider> logger)
        {
            this.providers = new IVersionControlProvider[] { bitBucketService, gitlabService };
            this.logger = logger;
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository)
        {
            List<Asset> results = new List<Asset>();

            foreach (IVersionControlProvider provider in providers)
            {
                IEnumerable<Asset> assets = await provider.GetAssetsAsync(repository);                

                results.AddRange(assets);
            }

            logger.LogInformation($"Found {results.Count} supported assets for repository: {repository.Id}");

            return results;
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl source)
        {
            List<Repository> results = new List<Repository>();

            foreach (IVersionControlProvider provider in providers)
            {
                IEnumerable<Repository> repositories = await provider.GetRepositoriesAsync(source);
                results.AddRange(repositories);
            }

            logger.LogInformation($"Found {results.Count} repositories for version control: {source.Endpoint}");

            return results;
        }
    }
}


