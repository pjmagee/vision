using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests.Unit.Pipelines
{
    public class TeamCityProviderTests
    {
        private readonly TeamCityProvider sut;
        private readonly VisionDbContext context;
        private readonly RepositoryMatcher repositoryMatcher;

        public TeamCityProviderTests()
        {
            var options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("TeamCity").UseLazyLoadingProxies().Options;
            context = new VisionDbContext(options);
            repositoryMatcher = new RepositoryMatcher(new LoggerFactory().CreateLogger<RepositoryMatcher>());
            sut = new TeamCityProvider(repositoryMatcher, new LoggerFactory().CreateLogger<TeamCityProvider>());
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
        public void Test()
        {
                      
        }
    }
}
