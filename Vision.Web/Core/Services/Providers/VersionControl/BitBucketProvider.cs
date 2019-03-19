namespace Vision.Web.Core
{
    using Atlassian.Stash;
    using Atlassian.Stash.Entities;
    using Atlassian.Stash.Helpers;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
        
        public async Task<IEnumerable<Asset>> GetAssetsAsync(VersionControlDto versionControl, Repository repository)
        {            
            List<Asset> results = new List<Asset>();
            StashClient client = new StashClient(versionControl.Endpoint, versionControl.ApiKey, usePersonalAccessTokenForAuthentication: true);

            ResponseWrapper<Project> projects = await client.Projects.Get();

            foreach (Project project in projects.Values ?? Enumerable.Empty<Project>())
            {
                ResponseWrapper<Atlassian.Stash.Entities.Repository> repositories = await client.Repositories.Get(project.Key, Options);

                foreach(Atlassian.Stash.Entities.Repository bitBucketRepository in repositories.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Repository>())
                {
                    bool isRepositoryFound = bitBucketRepository.Links.Clone.Select(c => c.Href).Concat(bitBucketRepository.Links.Self.Select(s => s.Href)).Any(link => matcher.IsMatch(link.ToString(), repository.Url));

                    if (isRepositoryFound)
                    {
                        ResponseWrapper<string> filePaths = await client.Repositories.GetFiles(project.Key, bitBucketRepository.Slug, Options);

                        foreach(string path in filePaths.Values ?? Enumerable.Empty<string>())
                        {
                            if (path.IsSupported())
                            {
                                Atlassian.Stash.Entities.File file  = await client.Repositories.GetFileContents(project.Key, bitBucketRepository.Slug, path, new FileContentsOptions { Content = true, Limit = 10000 });

                                logger.LogInformation($"Adding '{path}' for repository {repository.Id}");

                                results.Add(new Asset { Id = Guid.NewGuid(), Repository = repository, Kind = path.GetDependencyKind(), Path = path, Raw = string.Join(Environment.NewLine, file.FileContents) });
                            }
                        }
                    }
                }
            }

            return results;
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl versionControl)
        {  
            StashClient client = new StashClient(versionControl.Endpoint, versionControl.ApiKey, usePersonalAccessTokenForAuthentication: true);

            List<Repository> results = new List<Repository>();

            ResponseWrapper<Project> projects = await client.Projects.Get();

            foreach (Project project in projects.Values ?? Enumerable.Empty<Project>())
            {
                ResponseWrapper<Atlassian.Stash.Entities.Repository> repositories = await client.Repositories.Get(project.Key, Options);

                foreach (Atlassian.Stash.Entities.Repository repository in repositories.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Repository>())
                {
                    results.Add(new Repository
                    {
                        Id = Guid.NewGuid(),
                        VersionControl = versionControl,
                        VersionControlId = versionControl.Id,
                        WebUrl = repository.Links.Self[0].Href.ToString(),
                        Url = repository.Links.Clone[0].Href.ToString()
                    });
                }
            }

            logger.LogInformation($"Found {results.Count} repositories for {versionControl.Endpoint}");

            return results;
        }

        public bool Supports(VersionControlKind kind) => kind == VersionControlKind.Bitbucket;
    }
}
