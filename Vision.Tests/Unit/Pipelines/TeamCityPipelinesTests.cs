using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests.Unit.Pipelines
{
    public class TeamCityPipelinesTests
    {
        private readonly TeamCityBuildsService sut;
        private readonly VisionDbContext context;
        private readonly RepositoryMatcher repositoryMatcher;

        public TeamCityPipelinesTests()
        {
            var options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("TeamCity").UseLazyLoadingProxies().Options;
            context = new VisionDbContext(options);
            repositoryMatcher = new RepositoryMatcher(new LoggerFactory().CreateLogger<RepositoryMatcher>());
            sut = new TeamCityBuildsService(context, repositoryMatcher, new LoggerFactory().CreateLogger<TeamCityBuildsService>(), new DataProtectionStub());
        }

        [Theory]
        [InlineData(CiCdKind.Jenkins, false)]
        [InlineData(CiCdKind.Gitlab, false)]
        [InlineData(CiCdKind.TeamCity, true)]
        public void ShouldSupport(CiCdKind kind, bool expected)
        {
            Assert.Equal(expected: expected, actual: sut.Supports(kind));
        }

        [Fact]
        public async void Test()
        {
            var response = await sut.GetBuildsByRepositoryIdAsync(Guid.NewGuid());

            
        }
    }
}
