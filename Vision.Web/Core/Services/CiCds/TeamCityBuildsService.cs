using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{

    public class TeamCityBuildsService : ICICDBuildsService
    {

        private static readonly HttpClient BuildsClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });

        private readonly VisionDbContext context;
        private readonly IRepositoryMatcher matcher;
        private readonly ILogger<TeamCityBuildsService> logger;

        public TeamCityBuildsService(VisionDbContext context, IRepositoryMatcher matcher, ILogger<TeamCityBuildsService> logger)
        {
            this.context = context;
            this.matcher = matcher;
            this.logger = logger;
        }

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            List<CiCdBuildDto> builds = new List<CiCdBuildDto>();

            foreach (CiCd cicd in await context.CiCds.Where(buildSource => buildSource.Kind == CiCdKind.TeamCity).ToListAsync())
            {
                List<CiCdBuildDto> results = await GetTeamCityBuildsAsync(repository, cicd);
                builds.AddRange(results);
            }

            return builds;
        }

        private async Task<List<CiCdBuildDto>> GetTeamCityBuildsAsync(Repository repository, CiCd cicd)
        {
            var builds = new List<CiCdBuildDto>();

            try
            {
                BuildsClient.BaseAddress = new Uri(cicd.Endpoint + "guestAuth/app/rest/");

                var vcsRootsResponse = await BuildsClient.GetAsync("vcs-roots");
                var vcsRootsDocument = XDocument.Parse(await vcsRootsResponse.Content.ReadAsStringAsync());

                foreach (var vcsRootElement in vcsRootsDocument.Root.Elements("vcs-root"))
                {
                    var uri = vcsRootElement.Attribute("href")?.Value;

                    try
                    {
                        var vcsRootResponse = await BuildsClient.GetAsync(uri);
                        var vscRootDocument = XDocument.Parse(await vcsRootResponse.Content.ReadAsStringAsync());
                        var projectElement = vscRootDocument.Root.Element("project");
                        var vcsElement = vscRootDocument.Root.Element("properties").Elements("property").FirstOrDefault(p => p.Attribute("name").Value == "url");
                        var name = projectElement.Attribute("name").Value;
                        var webUri = projectElement.Attribute("webUrl").Value;
                        var vcsUrl = vcsElement?.Attribute("value").Value;

                        if (matcher.IsSameRepository(vcsUrl, repository.Url))
                        {
                            builds.Add(new CiCdBuildDto { Name = name, WebUrl = webUri, CiCdId = cicd.Id, Kind = cicd.Kind });
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Error finding build at {uri}");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error finding builds for {repository.WebUrl} at {cicd.Endpoint}");
            }

            return builds;
        }

        public bool Supports(CiCdKind Kind) => Kind == CiCdKind.TeamCity;
    }
}
