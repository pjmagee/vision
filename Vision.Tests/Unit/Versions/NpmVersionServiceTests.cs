namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using Vision.Web.Core;
    using Xunit;

    public class NpmVersionServiceTests : IDisposable
    {
        private readonly NpmVersionService sut;
        private readonly DbContextOptions<VisionDbContext> options;
        private readonly VisionDbContext context;

        public NpmVersionServiceTests()
        {
            options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
            context = new VisionDbContext(options);
            sut = new NpmVersionService(context, new DataProtectionStub(), new LoggerFactory().CreateLogger<NpmVersionService>());
        }

        [Theory]
        [InlineData("@angular/core", "7.0.1")]
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

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
