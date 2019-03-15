namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using Vision.Web.Core;
    using Xunit;

    public class NuGetVersionServiceTests : IDisposable
    {
        private readonly NuGetVersionService sut;
        private readonly DbContextOptions<VisionDbContext> options;
        private readonly VisionDbContext context;

        public NuGetVersionServiceTests()
        {
            options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
            context = new VisionDbContext(options);
            sut = new NuGetVersionService(context, new DataProtectionStub(), new LoggerFactory().CreateLogger<NuGetVersionService>());
        }

        [Theory]
        [InlineData("Atlassian.Stash.Api", "3.1.20")]
        public async Task NuGetV2Api(string package, string version)
        {
            // arrange
            context.Registries.Add(new Registry { Endpoint = "https://www.nuget.org/api/v2", IsEnabled = true, IsPublic = true, Kind = DependencyKind.NuGet });
            context.SaveChanges();

            var dependency = new Dependency { Name = package, Kind = DependencyKind.NuGet };

            // act
            var latest = await sut.GetLatestMetaDataAsync(dependency);

            // assert
            Assert.Equal(version, latest.Version);
        }

        [Theory]
        [InlineData("Atlassian.Stash.Api", "3.1.20")]
        public async Task NuGetV3Api(string package, string version)
        {
            // arrange
            context.Registries.Add(new Registry { Endpoint = "https://api.nuget.org/v3/index.json", IsEnabled = true, IsPublic = true, Kind = DependencyKind.NuGet });
            context.SaveChanges();

            var dependency = new Dependency { Name = package, Kind = DependencyKind.NuGet };

            // act
            var latest = await sut.GetLatestMetaDataAsync(dependency);

            // assert
            Assert.Equal(version, latest.Version);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
