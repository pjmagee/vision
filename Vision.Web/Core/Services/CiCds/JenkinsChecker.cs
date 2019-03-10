﻿namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.EntityFrameworkCore;

    public class JenkinsBuildsService : ICICDBuildsService
    {
        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });

        private readonly VisionDbContext context;
        private readonly IRepositoryMatcher matcher;

        public CiCdKind Kind => CiCdKind.Jenkins;

        public JenkinsBuildsService(VisionDbContext context, IRepositoryMatcher matcher)
        {
            this.context = context;
            this.matcher = matcher;
        }

        public async Task<IEnumerable<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
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
                HttpClient.BaseAddress = new Uri(cicd.Endpoint);
               
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

                        var vcsUrl = buildDocument.XPathSelectElement("//remoteUrl").Value;
                        var name = job.XPathSelectElement("name").Value;

                        if (matcher.IsSameRepository(vcsUrl, repository.Url))
                        {
                            builds.Add(new CiCdBuildDto { Name = name, WebUrl = jobUrl, CiCdId = cicd.Id, Kind = cicd.Kind });
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