namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class JenkinsBuildsService : ICICDBuildsService
    {
        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });

        private readonly VisionDbContext context;
        private readonly IRepositoryMatcher matcher;
        private readonly ILogger<JenkinsBuildsService> logger;

        public JenkinsBuildsService(VisionDbContext context, IRepositoryMatcher matcher, ILogger<JenkinsBuildsService> logger)
        {
            this.context = context;
            this.matcher = matcher;
            this.logger = logger;
        }

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            List<CiCdBuildDto> builds = new List<CiCdBuildDto>();

            foreach (var build in await context.CiCds.Where(buildSource => buildSource.Kind == CiCdKind.Jenkins).ToListAsync())
            {
                var results = await GetBuildsAsync(repository, build);
                builds.AddRange(results);
            }

            return builds;
        }

        public async Task<List<CiCdBuildDto>> GetBuildsAsync(Repository repository, CiCd cicd)
        {
            var builds = new List<CiCdBuildDto>();

            try
            {
                var query = cicd.Endpoint.Trim('/') + "/job/CI/api/xml";

                logger.LogInformation($"Checking {query} for builds related to {repository.WebUrl}");

                var jobXml = await HttpClient.GetStringAsync(query);
                var jobsDocument = XDocument.Parse(jobXml);
                var jobs = jobsDocument.XPathSelectElements("//job");

                foreach (var job in jobs)
                {
                    var jobUrl = job.XPathSelectElement("url").Value;

                    try
                    {
                        var buildUri = new Uri(new Uri(jobUrl).PathAndQuery + "/lastBuild/api/xml", UriKind.Relative);
                        var buildXml = await HttpClient.GetStringAsync(buildUri);
                        var buildDocument = XDocument.Parse(buildXml);

                        var vcsUrl = buildDocument.XPathSelectElement("//remoteUrl").Value;
                        var name = job.XPathSelectElement("name").Value;

                        if (matcher.IsMatch(vcsUrl, repository.Url))
                        {
                            builds.Add(new CiCdBuildDto { Name = name, WebUrl = jobUrl, CiCdId = cicd.Id, Kind = cicd.Kind });
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Error finding build at {job}");
                    }                    
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, $"Error finding builds for {repository.WebUrl} at {cicd.Endpoint}");
            }

            return builds;
        }

        public bool Supports(CiCdKind Kind) => Kind == CiCdKind.Jenkins;
    }
}
