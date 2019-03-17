namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Logging;

    public class JenkinsProvider : AbstractCICDProvider
    {
        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });
        public JenkinsProvider(VisionDbContext context, IRepositoryMatcher matcher, ILogger<JenkinsProvider> logger, IDataProtectionProvider provider) : base(context, provider, logger, matcher)
        {
            
        }

        protected override CiCdKind Kind => CiCdKind.Jenkins;

        protected override async Task<List<CiCdBuildDto>> GetBuildsAsync(Repository repository, CiCd cicd)
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

                        if (IsMatch(vcsUrl, repository.Url))
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
    }
}
