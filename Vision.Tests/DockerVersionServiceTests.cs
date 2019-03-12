using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Vision.Web.Core;
using Xunit;

namespace Vision.Tests
{
    public class DockerVersionServiceTests
    {
        private readonly DockerVersionService service;
        private readonly VisionDbContext context;

        public DockerVersionServiceTests()
        {
            var options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
            context = new VisionDbContext(options);
            service = new DockerVersionService(context, null, new LoggerFactory().CreateLogger<NuGetVersionService>());
        }

        [Theory]
        [InlineData("sonatype/nexus3", "latest")]
        public async Task DockerV2Api(string package, string version)
        {
            // arrange
            context.Registries.Add(new Registry { Endpoint = "nexus3.xpa.rbxd.ds:8080", ApiKey = "", IsEnabled = true, IsPublic = true, Kind = DependencyKind.Docker });
            context.SaveChanges();

            var dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = DependencyKind.NuGet };

            // act
            var latest = await service.GetLatestVersionAsync(dependency);

            // assert
            Assert.Equal(version, latest.Version);
        }
    }
}
