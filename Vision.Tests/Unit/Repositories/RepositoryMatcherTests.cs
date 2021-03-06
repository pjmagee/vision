﻿using Microsoft.Extensions.Logging;
using NSubstitute;
using Vision.Core;
using Xunit;

namespace Vision.Tests
{
    public class RepositoryMatcherTests
    {
        private readonly IRepositoryMatcher repositoryMatcher;

        public RepositoryMatcherTests()
        {
            repositoryMatcher = new RepositoryMatcher(Substitute.For<ILogger<RepositoryMatcher>>());
        }

        [Theory]
        [InlineData("ssh://git@domain:8080/key/project.git", "ssh://git@domain:8080/key/project.git", true)]
        [InlineData("http://domain:8090/scm/key/project.git", "http://domain:8090/scm/key/project.git", true)]
        [InlineData("http://domain:8090/scm/key/project.git", "ssh://git@domain:8080/key/project.git", true)]
        [InlineData("ssh://git@domain:8080/key/project.git", "http://domain:8090/scm/key/project.git", true)]
        [InlineData("ssh://git@domain:8080/key/project.git", "https://domain:8090/scm/key/project.git", true)]
        [InlineData("ssh://git@external:8080/key/project.git", "http://domain:8090/scm/key/project.git", false)]
        [InlineData("ssh://git@external:8080/key/project.git", "http://external:8090/scm/project2.git", false)]
        [InlineData("ssh://git@domain:8080/key/project.git", "ssh://git@domain:8080/key/project2.git", false)]
        public void MatcherTests(string one, string two, bool match)
        {
            Assert.Equal(expected: match, actual: repositoryMatcher.IsMatch(one, two));
        }
    }
}
