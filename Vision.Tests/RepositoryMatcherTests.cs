using Vision.Web.Core;
using Xunit;

namespace Vision.Tests
{
    public class RepositoryMatcherTests : IClassFixture<RepositoryMatcher>
    {
        private readonly IRepositoryMatcher matcher;

        public RepositoryMatcherTests(RepositoryMatcher matcher)
        {
            this.matcher = matcher;
        }

        [Fact]
        public void TwoSimilarRepositoriesAreAMatch()
        {
            Assert.True(matcher.IsSameRepository("ssh://git@stash.xpa.rbxd.ds:8080/fess/fess.git", "http://stash.xpa.rbxd.ds:8090/scm/fess/fess.git"));            
        }
    }
}
