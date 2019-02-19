using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.EntityFrameworkCore;
using Vision.Shared;

namespace Vision.Core.Services.Build
{
    public class JenkinsService : IBuildService
    {
        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });

        private readonly VisionDbContext context;

        public JenkinsService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Build>> GetBuildsByRepositoryAsync(Guid repositoryId)
        {
            GitRepository repository = await context.GitRepositories.FindAsync(repositoryId);

            List<Build> builds = new List<Build>();

            foreach (var build in await context.BuildSources.Where(buildSource => buildSource.Kind == BuildKind.Jenkins).ToListAsync())
            {
                var results = await GetBuildsAsync(repository.GitUrl, build.Endpoint, build.ApiKey);
                builds.AddRange(results);
            }

            return builds;
        }

        public async Task<List<Build>> GetBuildsAsync(string gitUrl, string endpoint, string apiKey)
        {
            var builds = new List<Build>();

            try
            {
                HttpClient.BaseAddress = new Uri(endpoint);
               
                var jobUri = new Uri("/job/CI/api/xml", UriKind.Relative);
                var jobXml = await HttpClient.GetStringAsync(jobUri);
                var jobsDocument = XDocument.Parse(jobXml);
                var jobs = jobsDocument.XPathSelectElements("//job");

                foreach (var job in jobs)
                {
                    try
                    {
                        var jobUrl = job.XPathSelectElement("url").Value;
                        var buildUri = new Uri(new Uri(jobUrl).PathAndQuery + "/lastBuild/api/xml", UriKind.Relative);
                        var buildXml = await HttpClient.GetStringAsync(buildUri);
                        var buildDocument = XDocument.Parse(buildXml);
                        var gitUri = buildDocument.XPathSelectElement("//remoteUrl").Value;
                        var name = job.XPathSelectElement("name").Value;

                        if(string.Equals(gitUri, gitUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            builds.Add(new Build { Name = name, GitUri = gitUri, WebUri = jobUrl });
                        }                  
                    }
                    catch (Exception)
                    {
                        
                    }                    
                }
            }
            catch(Exception)
            {

            }

            return builds;
        }
    }
}
