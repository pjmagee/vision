using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly IDataProtector protector;

        public TeamCityBuildsService(VisionDbContext context, IRepositoryMatcher matcher, ILogger<TeamCityBuildsService> logger, IDataProtectionProvider provider)
        {
            this.context = context;
            this.matcher = matcher;
            this.logger = logger;
            this.protector = provider.CreateProtector("Auth");
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

        private static string Base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        private async Task<List<CiCdBuildDto>> GetTeamCityBuildsAsync(Repository repository, CiCd cicd)
        {
            var builds = new List<CiCdBuildDto>();

            try
            {
                var authMode = cicd.IsGuestEnabled ? "guestAuth" : "httpAuth";                
                var query = cicd.Endpoint.Trim('/') + $"/{authMode}/app/rest/vcs-roots";

                logger.LogInformation($"Checking {query} for builds related to {repository.WebUrl}");

                if (!string.IsNullOrWhiteSpace(cicd.Username) && !string.IsNullOrWhiteSpace(cicd.Password))
                {
                    var username = Base64Encode(protector.Unprotect(cicd.Username));
                    var password = Base64Encode(protector.Unprotect(cicd.Password));

                    BuildsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", $"{username}:{password}");
                }

                var vcsRootsResponse = await BuildsClient.GetAsync(query, HttpCompletionOption.ResponseContentRead);

                var vcsRootsDocument = XDocument.Parse(await vcsRootsResponse.Content.ReadAsStringAsync());

                foreach (var vcsRootElement in vcsRootsDocument.Root.Elements("vcs-root"))
                {
                    var uri = vcsRootElement.Attribute("href")?.Value;

                    try
                    {
                        var vcsRootResponse = await BuildsClient.GetAsync(cicd.Endpoint.Trim('/') + uri);
                        var vscRootDocument = XDocument.Parse(await vcsRootResponse.Content.ReadAsStringAsync());
                        var projectElement = vscRootDocument.Root.Element("project");
                        var vcsElement = vscRootDocument.Root.Element("properties").Elements("property").FirstOrDefault(p => p.Attribute("name").Value == "url");
                        var name = projectElement.Attribute("name").Value;
                        var webUri = projectElement.Attribute("webUrl").Value;
                        var vcsUrl = vcsElement?.Attribute("value").Value;

                        if (matcher.IsMatch(vcsUrl, repository.Url))
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
