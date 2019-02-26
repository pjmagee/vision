using Atlassian.Stash;
using Atlassian.Stash.Entities;
using Atlassian.Stash.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core
{
    public class BitBuckerChecker : IVcsChecker
    {        
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly RequestOptions Options = new RequestOptions { Limit = 10000 };

        public async Task<IEnumerable<Asset>> GetAssetsAsync(Repository repository)
        {
            if (repository.VersionControl.Kind != VersionControlKind.Bitbucket) return Enumerable.Empty<Asset>();
            List<Asset> results = new List<Asset>();

            var base64 = repository.VersionControl.ApiKey;
            var client = new StashClient(repository.VersionControl.Endpoint, base64, usePersonalAccessTokenForAuthentication: true);

            ResponseWrapper<Project> projects = await client.Projects.Get();

            foreach (Project project in projects.Values ?? Enumerable.Empty<Project>())
            {
                ResponseWrapper<Atlassian.Stash.Entities.Repository> repositories = await client.Repositories.Get(project.Key, Options);

                foreach(Atlassian.Stash.Entities.Repository bitBucketRepository in repositories.Values ?? Enumerable.Empty<Atlassian.Stash.Entities.Repository>())
                {
                    bool isRepositoryFound = bitBucketRepository.CloneUrl == repository.Url;

                    if (isRepositoryFound)
                    {
                        ResponseWrapper<string> files = await client.Repositories.GetFiles(project.Key, bitBucketRepository.Slug, Options);

                        foreach(string file in files.Values ?? Enumerable.Empty<string>())
                        {
                            if (AppHelper.IsSupported(file))
                            {
                                string contents = await HttpClient.GetStringAsync(file);
                                results.Add(new Asset { Id = Guid.NewGuid(), Repository = repository, Path = file });
                            }
                        }
                    }
                }
            }

            return results;
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(VersionControl versionControl)
        {
            if (versionControl.Kind != VersionControlKind.Bitbucket) return Enumerable.Empty<Repository>();
            
            var base64 = versionControl.ApiKey;
            var client = new StashClient(versionControl.Endpoint, base64, usePersonalAccessTokenForAuthentication: true);

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
                        WebUrl = "TODO",
                        Url = "TODO"
                    });
                }
            }

            return results;
        }
    }
}
