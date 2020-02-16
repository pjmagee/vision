using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests.Unit.Pipelines
{
    public class JenkinsProviderTests
    {
        private readonly JenkinsProvider sut;
        private readonly RepositoryMatcher repositoryMatcher;

        public JenkinsProviderTests()
        {
            repositoryMatcher = new RepositoryMatcher(Substitute.For<ILogger<RepositoryMatcher>>());
            sut = new JenkinsProvider(repositoryMatcher, Substitute.For<ILogger<JenkinsProvider>>(), Substitute.For<IMemoryCache>());
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
        public void Test()
        {

        }
    }
}
