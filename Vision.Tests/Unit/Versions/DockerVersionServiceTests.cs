namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using Vision.Web.Core;
    using Xunit;

    public class DockerVersionServiceTests : IDisposable
    {
        private readonly DockerVersionService sut;
        private readonly VisionDbContext context;

        public DockerVersionServiceTests()
        {
            var options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
            context = new VisionDbContext(options);
            sut = new DockerVersionService(context, null, new LoggerFactory().CreateLogger<DockerVersionService>());
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
