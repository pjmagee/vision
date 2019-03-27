namespace Vision.Tests
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System.Threading.Tasks;
    using Vision.Web.Core;
    using Xunit;

    public class NuGetVersionProviderTests
    {
        private readonly NuGetVersionProvider sut;

        public NuGetVersionProviderTests()
        {
            sut = new NuGetVersionProvider(Substitute.For<ILogger<NuGetVersionProvider>>());
        }

        [Theory]
        [InlineData("https://www.nuget.org/api/v2", "Atlassian.Stash.Api", "3.1.20")]
        [InlineData("https://api.nuget.org/v3/index.json", "Atlassian.Stash.Api", "3.1.20")]

        public async Task NuGetApiTests(string endpoint, string package, string version)
        {
            // arrange
            RegistryDto registry = new RegistryDto { Endpoint = endpoint, IsEnabled = true, IsPublic = true, Kind = DependencyKind.NuGet };
            Dependency dependency = new Dependency { Name = package, Kind = DependencyKind.NuGet };

            // act
            DependencyVersion latest = await sut.GetLatestMetaDataAsync(registry, dependency);

            // assert
            Assert.Equal(version, latest.Version);
        }
    }
}
