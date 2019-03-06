using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Vision.Core;
using Xunit;

namespace Vision.Tests
{
    public class NuGetVersionServiceTests
    {
        private readonly NuGetVersionService service;

        private readonly DbContextOptions<VisionDbContext> options;

        private readonly VisionDbContext context;

        public NuGetVersionServiceTests()
        {
            options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
            context = new VisionDbContext(options);
            service = new NuGetVersionService(context);
        }

        [Theory]
        [InlineData("Atlassian.Stash.Api", "3.1.20" )]        
        public async Task NuGetV2Api(string package, string version)
        {
            // arrange
            context.Registries.Add(new Registry { Endpoint = "https://www.nuget.org/api/v2", ApiKey = "", IsEnabled = true, IsPublic = true, Kind = Shared.DependencyKind.NuGet });
            context.SaveChanges();
           
            var dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = Shared.DependencyKind.NuGet };

            // act
            var latest = await service.GetLatestVersionAsync(dependency);

            // assert
            Assert.Equal(version, latest.Version);
        }

        [Theory]
        [InlineData("Atlassian.Stash.Api", "3.1.20")]
        public async Task NuGetV3Api(string package, string version)
        {
            context.Registries.Add(new Registry { Endpoint = "https://api.nuget.org/v3/index.json", ApiKey = "", IsEnabled = true, IsPublic = true, Kind = Shared.DependencyKind.NuGet });
            context.SaveChanges();

            var dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = Shared.DependencyKind.NuGet };

            // act
            var latest = await service.GetLatestVersionAsync(dependency);

            // assert
            Assert.Equal(version, latest.Version);
        }
    }
}
