using Microsoft.Extensions.Logging;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests
{
    public class RepositoryMatcherTests
    {
        private readonly IRepositoryMatcher repositoryMatcher;

        public RepositoryMatcherTests()
        {
            this.repositoryMatcher = new RepositoryMatcher(new LoggerFactory().CreateLogger<RepositoryMatcher>());
        }

        [Fact]
        public void SshToHttpIsMatch()
        {
            // arrange
            var one = "ssh://git@stash.xpa.rbxd.ds:8080/fess/fess.git";
            var two = "http://stash.xpa.rbxd.ds:8090/scm/fess/fess.git";

            // act
            var isMatch = repositoryMatcher.IsMatch(one, two);

            Assert.True(isMatch);            
        }

        [Fact]
        public void HttpToSshIsMatch()
        {
            // arrange            
            var one = "http://stash.xpa.rbxd.ds:8090/scm/fess/fess.git";
            var two = "ssh://git@stash.xpa.rbxd.ds:8080/fess/fess.git";

            // act
            var isMatch = repositoryMatcher.IsMatch(one, two);

            Assert.True(isMatch);
        }

        [Fact]
        public void HttpToHttpIsMatch()
        {
            // arrange            
            var one = "http://stash.xpa.rbxd.ds:8090/scm/fess/fess.git";
            var two = "http://stash.xpa.rbxd.ds:8090/scm/fess/fess.git";

            // act
            var isMatch = repositoryMatcher.IsMatch(one, two);

            Assert.True(isMatch);
        }

        [Fact]
        public void SshToSshIsMatch()
        {
            // arrange            
            var one = "ssh://git@stash.xpa.rbxd.ds:8080/fess/fess.git";
            var two = "ssh://git@stash.xpa.rbxd.ds:8080/fess/fess.git";

            // act
            var isMatch = repositoryMatcher.IsMatch(one, two);

            Assert.True(isMatch);
        }
    }
}
