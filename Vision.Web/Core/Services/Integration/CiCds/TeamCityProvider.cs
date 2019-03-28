using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public class TeamCityProvider : AbstractCICDProvider
    {
        public TeamCityProvider(IRepositoryMatcher matcher, ILogger<TeamCityProvider> logger, IMemoryCache memoryCache) : base(logger, matcher, memoryCache)
        {

        }

        protected override CiCdKind Kind => CiCdKind.TeamCity;

        protected override void AddAuthentication(HttpClient client, CiCdDto cicd)
        {
            if (!string.IsNullOrWhiteSpace(cicd.Username) && !string.IsNullOrWhiteSpace(cicd.Password))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Base64Encode($"{cicd.Username}:{cicd.Password}"));
            }
        }

        protected override async Task<List<CiCdBuildDto>> TryGetBuildsAsync(RepositoryDto repository, CiCdDto cicd)
        {
            var builds = new List<CiCdBuildDto>();

            try
            {
                var client = GetHttpClient(cicd);
                var authMode = cicd.IsGuestEnabled ? "guestAuth" : "httpAuth";                
                var query = cicd.Endpoint.Trim('/') + $"/{authMode}/app/rest/vcs-roots";

                logger.LogTrace($"Checking {query} for builds related to {repository.WebUrl}");

                var vcsRootsResponse = await client.GetAsync(query, HttpCompletionOption.ResponseContentRead);
                var vcsRootsDocument = XDocument.Parse(await vcsRootsResponse.Content.ReadAsStringAsync());

                foreach (var vcsRootElement in vcsRootsDocument.Root.Elements("vcs-root"))
                {
                    var uri = vcsRootElement.Attribute("href")?.Value;

                    try
                    {
                        var vcsRootResponse = await client.GetAsync(cicd.Endpoint.Trim('/') + uri);
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
