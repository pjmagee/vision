using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Vision.Core;
using Xunit;

namespace Vision.Tests
{
    public class NpmExtractorTests
    {
        private readonly NpmAssetExtractor sut;

        public NpmExtractorTests()
        {
            sut = new NpmAssetExtractor(Substitute.For<ILogger<NpmAssetExtractor>>());
        }

        [Fact]
        public void ExtractsDependencies()
        {
            // arrange
            string json = "{ \"name\": \"Vision\", \"version\": \"0.0.0\",  \"dependencies\": { \"express\": \"expressjs/express\", \"mocha\": \"mochajs/mocha#4727d357ea\", \"module\": \"user/repo#feature\\/branch\" } }";
            Asset asset = new Asset { Repository = new VcsRepository { WebUrl = "http://git:8080/KEY/repository.git/Browse/" }, Raw = json };

            // act
            List<Extract> extracts = sut.ExtractDependencies(asset).ToList();

            Assert.Equal(3, extracts.Count);
            Assert.Equal("express", extracts[0].EcosystemIdentifier);
            Assert.Equal("expressjs/express", extracts[0].EcosystemVersion);
            Assert.Equal("mocha", extracts[1].EcosystemIdentifier);
            Assert.Equal("mochajs/mocha#4727d357ea", extracts[1].EcosystemVersion);
        }

        [Fact]
        public void ExtractRuntimes()
        {
            // Arrange
            string json = "{ \"name\": 'Vision', \"engines\": { \"node\":\">=0.10.3 <0.12\", \"npm\":\"10.0\" } } ";
            Asset asset = new Asset { Repository = new VcsRepository { WebUrl = "http://git:8080/KEY/repository.git/Browse/" }, Raw = json };

            // Act
            List<Extract> extracts = sut.ExtractEcoSystem(asset).ToList();

            Assert.Equal(2, extracts.Count);
            Assert.Equal("node", extracts[0].EcosystemIdentifier);
            Assert.Equal(">=0.10.3 <0.12", extracts[0].EcosystemVersion);
            Assert.Equal("npm", extracts[1].EcosystemIdentifier);
            Assert.Equal("10.0", extracts[1].EcosystemVersion);
        }

        [Fact]
        public void ExtractsName()
        {
            // Arrange
            string json = "{ \"name\": 'Vision', \"engines\": { \"node\":\">=0.10.3 <0.12\", \"npm\":\"10.0\" } } ";
            Asset asset = new Asset { Repository = new VcsRepository { WebUrl = "http://git:8080/KEY/repository.git/Browse/" }, Raw = json };

            // Act
            string name = sut.ExtractPublishName(asset);

            Assert.Equal("Vision", name);
        }
    }
}
