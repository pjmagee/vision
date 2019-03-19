using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class TeamCityProvider : AbstractCICDProvider
    {
        private static readonly HttpClient BuildsClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });

        public TeamCityProvider(IRepositoryMatcher matcher, ILogger<TeamCityProvider> logger) : base(logger, matcher)
        {

        }

        protected override CiCdKind Kind => CiCdKind.TeamCity;

        protected override async Task<List<CiCdBuildDto>> TryGetBuildsAsync(RepositoryDto repository, CiCdDto cicd)
        {
            var builds = new List<CiCdBuildDto>();

            try
            {
                var authMode = cicd.IsGuestEnabled ? "guestAuth" : "httpAuth";                
                var query = cicd.Endpoint.Trim('/') + $"/{authMode}/app/rest/vcs-roots";

                logger.LogTrace($"Checking {query} for builds related to {repository.WebUrl}");

                if (!string.IsNullOrWhiteSpace(cicd.Username) && !string.IsNullOrWhiteSpace(cicd.Password))
                {
                    BuildsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", $"{cicd.Username}:{cicd.Password}");
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
                            builds.Add(new CiCdBuildDto { Name = name, WebUrl = webUri, CiCdId = cicd.CiCdId, Kind = cicd.Kind });
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogTrace(e, $"Error finding build at {uri}");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogTrace(e, $"Error finding builds for {repository.WebUrl} at {cicd.Endpoint}");
            }

            return builds;
        }
    }
}
