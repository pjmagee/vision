namespace Vision.Tests
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;
    using System.Threading.Tasks;
    using Vision.Web.Core;
    using Xunit;

    public class PyPiVersionProviderTests
    {
        private readonly PyPiVersionProvider sut;

        public PyPiVersionProviderTests()
        {
            sut = new PyPiVersionProvider(Substitute.For<ILogger<PyPiVersionProvider>>());
        }

        [Theory]
        [InlineData("pymongo", "3.7.2")]
        public async Task DockerV2Api(string package, string expected)
        {
            // arrange
            RegistryDto registry = new RegistryDto { Endpoint = "https://nexus3.xpa.rbxd.ds/repository/pypi-group/", IsEnabled = true, IsPublic = false, Kind = DependencyKind.PyPi };
            Dependency dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = DependencyKind.NuGet };

            // act
            DependencyVersion latest = await sut.GetLatestMetaDataAsync(registry, dependency);

            // assert
            Assert.Equal(expected, latest.Version);
        }
    }
}
