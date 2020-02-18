using Atlassian.Stash;
using Atlassian.Stash.Entities;
using Atlassian.Stash.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Core
{
    public class BitBucketProvider : IVersionControlProvider
    {
        private static readonly RequestOptions Options = new RequestOptions { Limit = 10000 };
        private readonly IRepositoryMatcher matcher;
        private readonly ILogger<BitBucketProvider> logger;

        public BitBucketProvider(IRepositoryMatcher matcher, ILogger<BitBucketProvider> logger)
        {
            this.matcher = matcher;
            this.logger = logger;
        }

        public VcsKind Kind => VcsKind.Bitbucket;

        public async Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, RepositoryDto repository)
        {
            List<Asset> results = new List<Asset>();
            StashClient client = new StashClient(versionControl.Endpoint, versionControl.ApiKey, usePersonalAccessTokenForAuthentication: true);

            ResponseWrapper<Project> projects = await client.Projects.Get();

            foreach (Project project in projects.Values ?? Enumerable.Empty<Project>())
            {
                ResponseWrapper<Repository> repositories = await client.Repositories.Get(project.Key, Options);

                foreach (Repository bitBucketRepository in repositories.Values ?? Enumerable.Empty<Repository>())
                {
                    bool isRepositoryFound = bitBucketRepository.Links.Clone.Select(c => c.Href).Concat(bitBucketRepository.Links.Self.Select(s => s.Href)).Any(link => matcher.IsMatch(link.ToString(), repository.Url));

                    if (isRepositoryFound)
                    {
                        ResponseWrapper<string> filePaths = await client.Repositories.GetFiles(project.Key, bitBucketRepository.Slug, Options);

                        foreach (string path in filePaths.Values ?? Enumerable.Empty<string>())
                        {
                            if (path.IsSupported())
                            {
                                File file = await client.Repositories.GetFileContents(project.Key, bitBucketRepository.Slug, path, new FileContentsOptions { Content = true, Limit = 10000 });

                                logger.LogInformation($"Adding '{path}' for repository {repository.RepositoryId}");

                                results.Add(new Asset { Id = Guid.NewGuid(), RepositoryId = repository.RepositoryId, Kind = path.GetEcosystemKind(), Path = path, Raw = string.Join(Environment.NewLine, file.FileContents) });
                            }
                        }
                    }
                }
            }

            return results;
        }

        public async Task<IEnumerable<VcsRepository>> GetRepositoriesAsync(VersionControlDto versionControl)
        {
            StashClient client = new StashClient(versionControl.Endpoint, versionControl.ApiKey, usePersonalAccessTokenForAuthentication: true);

            List<VcsRepository> results = new List<VcsRepository>();

            ResponseWrapper<Project> projects = await client.Projects.Get();

            foreach (Project project in projects.Values ?? Enumerable.Empty<Project>())
            {
                ResponseWrapper<Repository> repositories = await client.Repositories.Get(project.Key, Options);

                foreach (Repository repository in repositories.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Repository>())
                {
                    results.Add(new VcsRepository
                    {
                        Id = Guid.NewGuid(),
                        VcsId = versionControl.VcsId,
                        WebUrl = repository.Links.Self[0].Href.ToString(),
                        Url = repository.Links.Clone[0].Href.ToString()
                    });
                }
            }

            logger.LogInformation($"Found {results.Count} repositories for {versionControl.Endpoint}");

            return results;
        }
    }
}
