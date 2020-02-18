using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;
using Vision.Core;
using Xunit;

namespace Vision.Tests
{
    public class NpmVersionProviderTests
    {
        private readonly NpmVersionProvider sut;

        public NpmVersionProviderTests()
        {
            sut = new NpmVersionProvider(Substitute.For<ILogger<NpmVersionProvider>>());
        }

        [Theory]
        [InlineData("https://registry.npmjs.org/", "@angular/core", "7.2.10")]
        public async Task NpmApiTest(string endpoint, string package, string expected)
        {
            // arrange
            RegistryDto registry = new RegistryDto { Endpoint = endpoint, IsEnabled = true, IsPublic = true, Kind = EcosystemKind.Npm };
            Dependency dependency = new Dependency { Name = package, Kind = EcosystemKind.Npm };

            // act
            DependencyVersion latest = await sut.GetLatestMetaDataAsync(registry, dependency);

            // assert
            Assert.Equal(expected, latest.Version);
        }
    }
}
