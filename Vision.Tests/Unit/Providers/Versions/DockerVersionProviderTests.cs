using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Vision.Core;
using Xunit;

namespace Vision.Tests
{
    public class DockerVersionProviderTests
    {
        private readonly DockerVersionProvider sut;

        public DockerVersionProviderTests()
        {
            sut = new DockerVersionProvider(Substitute.For<ILogger<DockerVersionProvider>>());
        }

        [Theory]
        [InlineData("sonatype/nexus3", "latest")]
        public async Task DockerV2Api(string package, string expected)
        {
            // arrange
            RegistryDto registry = new RegistryDto { Endpoint = "https://registry-1.docker.io/v2/", IsEnabled = true, IsPublic = true, Kind = EcosystemKind.Docker };

            Dependency dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = EcosystemKind.NuGet };

            // act
            DependencyVersion latest = await sut.GetLatestMetaDataAsync(registry, dependency);

            // assert
            Assert.Equal(expected, latest.Version);
        }
    }
}
