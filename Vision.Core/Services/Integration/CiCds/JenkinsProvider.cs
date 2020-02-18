namespace Vision.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class JenkinsProvider : AbstractCICDProvider
    {
        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });
        public JenkinsProvider(IRepositoryMatcher matcher, ILogger<JenkinsProvider> logger, IMemoryCache memoryCache) : base(logger, matcher, memoryCache)
        {
            
        }

        protected override CiCdKind Kind => CiCdKind.Jenkins;

        protected override void AddAuthentication(HttpClient client, CiCdDto cicd)
        {
            bool UserAndApiKey = !string.IsNullOrWhiteSpace(cicd.Username) && !string.IsNullOrWhiteSpace(cicd.ApiKey);

            if (UserAndApiKey)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", Base64Encode($"{cicd.Username}:{cicd.ApiKey}"));
            }
            else
            {
                bool UserAndPassword = !string.IsNullOrWhiteSpace(cicd.Username) && !string.IsNullOrWhiteSpace(cicd.Password);

                if (UserAndPassword)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", Base64Encode($"{cicd.Username}:{cicd.Password}"));
                }
            }
        }

        protected override async Task<List<CiCdBuildDto>> TryGetBuildsAsync(RepositoryDto repository, CiCdDto cicd)
        {
            var results = new List<CiCdBuildDto>();

            try
            {
                HttpClient httpClient = GetHttpClient(cicd);               

                logger.LogTrace($"Checking {cicd.Endpoint} for builds related to {repository.WebUrl}");

                var jobs = await GetJobs(httpClient, new Uri(cicd.Endpoint));
                var builds = await GetBuilds(httpClient, jobs);

                foreach (var build in builds)
                {
                    try
                    {
                        var buildInfo = XDocument.Parse(await HttpClient.GetStringAsync(new Uri(new Uri(build, UriKind.Absolute), "api/xml")));
                        var vcsUrl = buildInfo.XPathSelectElement("//remoteUrl").Value;
                        var name = buildInfo.XPathSelectElement("//fullDisplayName").Value;
                        var webUrl = buildInfo.XPathSelectElement("//url").Value;

                        if (matcher.IsMatch(vcsUrl, repository.Url))
                        {
                            results.Add(new CiCdBuildDto { Name = name, WebUrl = webUrl, CiCdId = cicd.CiCdId, Kind = cicd.Kind });
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogTrace(e, $"Error finding build at {build}");
                    }                    
                }               
            }
            catch(Exception e)
            {
                logger.LogTrace(e, $"Error finding builds for {repository.WebUrl} at {cicd.Endpoint}");
            }

            return results;
        }

        private async Task<IEnumerable<string>> GetBuilds(HttpClient client, IEnumerable<string> jobs)
        {
            var builds = new List<string>();

            foreach (var job in jobs)
            {
                var xml = XDocument.Parse(await client.GetStringAsync(new Uri(new Uri(job, UriKind.Absolute), "api/xml")));

                var build = xml.XPathSelectElement("//build/url")?.Value;

                if (build != null)
                {
                    builds.Add(build);
                }
            }

            return builds;
        }

        private async Task<IEnumerable<string>> GetJobs(HttpClient client, Uri url)
        {
            var list = new List<string>();
            var xml = XDocument.Parse(await client.GetStringAsync(new Uri(url, "api/xml")));

            var jobs = xml.XPathSelectElements("//job/url").Select(x => x.Value).ToList();

            if (jobs.Any())
            {
                list.AddRange(jobs);
                var jobTasks = jobs.Select(async job => await GetJobs(client, new Uri(job)));
                var results = await Task.WhenAll(jobTasks.ToArray());
                jobs.AddRange(results.SelectMany(x => x));
            }

            return jobs;
        }
    }
}
