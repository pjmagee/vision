using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Vision.Shared;

namespace Vision.Core.Services.Builds
{
    public class TeamCityService : IBuildService
    {        
        private static readonly HttpClient BuildsClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });

        private readonly VisionDbContext context;

        public TeamCityService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Build>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            GitRepository repository = await context.GitRepositories.FindAsync(repositoryId);

            List<Build> builds = new List<Build>();

            foreach (var build in await context.BuildSources.Where(buildSource => buildSource.Kind == Shared.BuildKind.TeamCity).ToListAsync())
            {
                var results = await GetTeamCityBuildsAsync(repository.GitUrl, build.Endpoint, build.ApiKey);
                builds.AddRange(results);
            }

            return builds;
        }

        private async Task<List<Build>> GetTeamCityBuildsAsync(string repositoryGitUrl, string endpoint, string apiKey)
        {
            var builds = new List<Build>();

            try
            {
                // API auth ??
                var vcsRootsResponse = await BuildsClient.GetAsync(endpoint + "/app/rest/vcs-roots");
                var vcsRootsDocument = XDocument.Parse(await vcsRootsResponse.Content.ReadAsStringAsync());

                foreach (var vcsRootElement in vcsRootsDocument.Root.Elements("vcs-root"))
                {
                    var uri = endpoint + vcsRootElement.Attribute("href")?.Value;

                    try
                    {
                        var vcsRootResponse = await BuildsClient.GetAsync(uri);
                        var vscRootDocument = XDocument.Parse(await vcsRootResponse.Content.ReadAsStringAsync());
                        var projectElement = vscRootDocument.Root.Element("project");
                        var gitElement = vscRootDocument.Root.Element("properties").Elements("property").FirstOrDefault(p => p.Attribute("name").Value == "url");
                        var name = projectElement.Attribute("name").Value;
                        var webUri = projectElement.Attribute("webUrl").Value;
                        var gitUri = gitElement?.Attribute("value").Value;

                        if (repositoryGitUrl == gitUri)
                        {
                            builds.Add(new Build { Name = name, WebUrl = webUri });
                        }
                    }
                    catch (Exception)
                    {
                       
                    }
                }
            }
            catch (Exception)
            {
                
            }

            return builds;
        }
    }
}
