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
    public class TeamCityChecker : ICiCdChecker
    {        
        private static readonly HttpClient BuildsClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });

        private readonly VisionDbContext context;
        private readonly IRepositoryMatcher matcher;

        public TeamCityChecker(VisionDbContext context, IRepositoryMatcher matcher)
        {
            this.context = context;
            this.matcher = matcher;
        }

        public async Task<IEnumerable<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
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
                BuildsClient.BaseAddress = new Uri(cicd.Endpoint);

                // API auth ??
                var vcsRootsResponse = await BuildsClient.GetAsync("/app/rest/vcs-roots");
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
