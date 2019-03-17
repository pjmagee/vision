using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests.Unit.Pipelines
{
    public class JenkinsProviderTests
    {
        private readonly JenkinsProvider sut;
        private readonly VisionDbContext context;
        private readonly RepositoryMatcher repositoryMatcher;

        public JenkinsProviderTests()
        {
            var options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Jenkins").UseLazyLoadingProxies().Options;
            context = new VisionDbContext(options);
            repositoryMatcher = new RepositoryMatcher(new LoggerFactory().CreateLogger<RepositoryMatcher>());
            sut = new JenkinsProvider(context, repositoryMatcher, new LoggerFactory().CreateLogger<JenkinsProvider>(), new DataProtectionStub());
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
