using Atlassian.Stash;
using Atlassian.Stash.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public class BitBucketService : IGitService
    {        
        private static HttpClient httpClient = new HttpClient();

        private static readonly RequestOptions options = new RequestOptions { Limit = 10000 };

        public BitBucketService()
        {
            
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync(GitRepository repository)
        {
            if (repository.GitSource.Kind != GitKind.Bitbucket) return Enumerable.Empty<Asset>();
            List<Asset> results = new List<Asset>();

            var base64 = repository.GitSource.ApiKey;
            var client = new StashClient(repository.GitSource.Endpoint, base64, usePersonalAccessTokenForAuthentication: true);

            ResponseWrapper<Atlassian.Stash.Entities.Project> projects = await client.Projects.Get();

            foreach (Atlassian.Stash.Entities.Project project in projects.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Project>())
            {
                ResponseWrapper<Atlassian.Stash.Entities.Repository> repositories = await client.Repositories.Get(project.Key, options);

                foreach(Atlassian.Stash.Entities.Repository bitBucketRepository in repositories.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Repository>())
                {
                    bool isRepositoryFound = bitBucketRepository.CloneUrl == repository.GitUrl;

                    if (isRepositoryFound)
                    {
                        ResponseWrapper<string> files = await client.Repositories.GetFiles(project.Key, bitBucketRepository.Slug, options);

                        foreach(string file in files.Values ?? Enumerable.Empty<string>())
                        {
                            if (AssetHelper.IsSupported(file))
                            {
                                string contents = await httpClient.GetStringAsync(file);
                                results.Add(new Asset { Id = Guid.NewGuid(), GitRepository = repository, Path = file });
                            }
                        }
                    }
                }
            }

            return results;
        }

        public async Task<IEnumerable<GitRepository>> GetRepositoriesAsync(GitSource source)
        {
            if (source.Kind != GitKind.Bitbucket) return Enumerable.Empty<GitRepository>();
            
            var base64 = source.ApiKey;
            var client = new StashClient(source.Endpoint, base64, usePersonalAccessTokenForAuthentication: true);

            List<GitRepository> results = new List<GitRepository>();

            ResponseWrapper<Atlassian.Stash.Entities.Project> projects = await client.Projects.Get();

            foreach (Atlassian.Stash.Entities.Project project in projects.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Project>())
            {
                ResponseWrapper<Atlassian.Stash.Entities.Repository> repositories = await client.Repositories.Get(project.Key, options);

                foreach (Atlassian.Stash.Entities.Repository repository in repositories.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Repository>())
                {
                    results.Add(new GitRepository
                    {
                        Id = Guid.NewGuid(),
                        GitSource = source,
                        GitSourceId = source.Id,
                        WebUrl = "TODO",
                        GitUrl = "TODO"
                    });
                }
            }

            return results;
        }
    }
}
