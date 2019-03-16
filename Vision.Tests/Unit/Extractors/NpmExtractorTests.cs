using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests
{
    public class NpmExtractorTests
    {
        private readonly NpmAssetExtractor sut;

        public NpmExtractorTests()
        {
            sut = new NpmAssetExtractor(new LoggerFactory().CreateLogger<NpmAssetExtractor>());
        }

        [Fact]
        public void ExtractsDependencies()
        {
            // arrange
            string json = "{ \"name\": \"Vision\", \"version\": \"0.0.0\",  \"dependencies\": { \"express\": \"expressjs/express\", \"mocha\": \"mochajs/mocha#4727d357ea\", \"module\": \"user/repo#feature\\/branch\" } }";
            Asset asset = new Asset { Repository = new Repository { WebUrl = "http://git:8080/KEY/repository.git/Browse/" }, Raw = json };

            // act
            List<Extract> extracts = sut.ExtractDependencies(asset).ToList();

            Assert.Equal(3, extracts.Count);
            Assert.Equal("express", extracts[0].Name);
            Assert.Equal("expressjs/express", extracts[0].Version);
            Assert.Equal("mocha", extracts[1].Name);
            Assert.Equal("mochajs/mocha#4727d357ea", extracts[1].Version);
        }

        [Fact]
        public void ExtractsFrameworks()
        {
            // Arrange
            string json = "{ \"name\": 'Vision', \"engines\": { \"node\":\">=0.10.3 <0.12\", \"npm\":\"10.0\" } } ";
            Asset asset = new Asset { Repository = new Repository { WebUrl = "http://git:8080/KEY/repository.git/Browse/" }, Raw = json };

            // Act
            List<Extract> extracts = sut.ExtractFrameworks(asset).ToList();

            Assert.Equal(2, extracts.Count);
            Assert.Equal("node", extracts[0].Name);
            Assert.Equal(">=0.10.3 <0.12", extracts[0].Version);
            Assert.Equal("npm", extracts[1].Name);
            Assert.Equal("10.0", extracts[1].Version);
        }

        [Fact]
        public void ExtractsName()
        {
            // Arrange
            string json = "{ \"name\": 'Vision', \"engines\": { \"node\":\">=0.10.3 <0.12\", \"npm\":\"10.0\" } } ";
            Asset asset = new Asset { Repository = new Repository { WebUrl = "http://git:8080/KEY/repository.git/Browse/" }, Raw = json };

            // Act
            string name = sut.ExtractPublishName(asset);

            Assert.Equal("Vision", name);
        }
    }
}
