using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests.Unit.Pipelines
{
    public class JenkinsPipelinesTests
    {
        private readonly JenkinsBuildsService sut;
        private readonly VisionDbContext context;
        private readonly RepositoryMatcher repositoryMatcher;

        public JenkinsPipelinesTests()
        {
            var options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Jenkins").UseLazyLoadingProxies().Options;
            context = new VisionDbContext(options);
            repositoryMatcher = new RepositoryMatcher(new LoggerFactory().CreateLogger<RepositoryMatcher>());
            sut = new JenkinsBuildsService(context, repositoryMatcher, new LoggerFactory().CreateLogger<JenkinsBuildsService>(), new DataProtectionStub());
        }

        [Theory]        
        [InlineData(CiCdKind.Gitlab, false)]
        [InlineData(CiCdKind.TeamCity, false)]
        [InlineData(CiCdKind.Jenkins, true)]
        public void ShouldSupport(CiCdKind kind, bool expected)
        {
            Assert.Equal(expected: expected, actual: sut.Supports(kind));
        }

        [Fact]
        public async void Test()
        {
            
        }
    }
}
