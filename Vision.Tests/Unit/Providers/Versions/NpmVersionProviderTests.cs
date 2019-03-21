namespace Vision.Tests
{
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;
    using System.Threading.Tasks;
    using Vision.Web.Core;
    using Xunit;

    public class NpmVersionProviderTests : IDisposable
    {
        private readonly NpmVersionProvider sut;
        private readonly DbContextOptions<VisionDbContext> options;
        private readonly VisionDbContext context;

        public NpmVersionProviderTests()
        {
            options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
            context = new VisionDbContext(options);
            sut = new NpmVersionProvider(Substitute.For<ILogger<NpmVersionProvider>>());
        }

        [Theory]
        [InlineData("https://registry.npmjs.org/", "@angular/core", "7.2.10")]
        public async Task NpmApiTest(string endpoint, string package, string expected)
        {
            // arrange
            RegistryDto registry = new RegistryDto { Endpoint = endpoint, IsEnabled = true, IsPublic = true, Kind = DependencyKind.Npm };
            Dependency dependency = new Dependency { Name = package, Kind = DependencyKind.Npm };

            // act
            DependencyVersion latest = await sut.GetLatestMetaDataAsync(registry, dependency);

            // assert
            Assert.Equal(expected, latest.Version);
        }

        public void Dispose() => context?.Dispose();
    }
}
