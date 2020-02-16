namespace Vision.Tests
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Vision.Web.Core;
    using Xunit;

    public class PyPiAssetExtractorTests
    {
        private readonly PyPiAssetExtractor sut;

        public PyPiAssetExtractorTests()
        {
            sut = new PyPiAssetExtractor(Substitute.For<ILogger<PyPiAssetExtractor>>());
        }

        [Theory]
        [MemberData(nameof(InlineImages))]
        public void ExtractMultiImages(string file, string[] packages, string[] versions)
        {
            var dependencies = sut.ExtractDependencies(new Asset { Raw = file }).ToList();

            for (int i = 0; i < dependencies.Count; i++)
            {
                Assert.Equal(packages[i], dependencies[i].RuntimeIdentifier);
                Assert.Equal(versions[i], dependencies[i].RuntimeVersion);
            }

            Assert.Equal(packages.Length, dependencies.Count);
        }

        public static IEnumerable<object[]> InlineImages => new List<object[]>
        {
            new object[]
            {
                "a==1.0.0" + Environment.NewLine + "b>=2.0.0" + Environment.NewLine + "c<=3.0.0" + Environment.NewLine + "d~=4.0.0",
                new[] { "a", "b", "c", "d" },
                new[] { "1.0.0", "2.0.0", "3.0.0", "4.0.0" }
            }
        };
    }
}
