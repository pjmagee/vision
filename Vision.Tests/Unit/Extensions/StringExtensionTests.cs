namespace Vision.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Vision.Web.Core;
    using Xunit;

    public class StringExtensionTests
    {
        public static IEnumerable<object[]> Extensions => AppHelper
            .SupportedExtensions
            .Select(ext => new object[] { ext, true })
            .Concat(new[]
            {
                new object[] { ".sample", false },
                new object[] { ".proj.sample", false },                
                new object[] { "Dockerfile.sample", false },
                new object[] { "pom.xml.sample", false },
                new object[] { "node_modules/package.json", false },
                new object[] { "/node_modules/package.json", false }
            });

        [Theory]
        [MemberData(nameof(Extensions))]
        public void ExtensionsShouldBeSupported(string extension, bool expected)
        {
            Assert.Equal(expected: expected, actual: extension.IsSupported());
        }

        [Theory]
        [InlineData(".sample")]
        [InlineData(".csproj.sample")]
        [InlineData("Dockerfile.sample")]
        [InlineData("packages.json.sample")]
        [InlineData("sample.sln")]
        [InlineData("maven.pom.xml.sample")]           
        public void GetEcosystemKindShouldThrowExceptionOnInvalidFile(string unsupported)
        {
            Assert.Throws<InvalidOperationException>(() => unsupported.GetEcosystemKind());
        }
    }
}
