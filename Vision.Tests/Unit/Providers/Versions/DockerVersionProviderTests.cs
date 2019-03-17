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

    public class DockerVersionProviderTests : IDisposable
    {
        private readonly DockerVersionProvider sut;
        private readonly VisionDbContext context;

        public DockerVersionProviderTests()
        {
            var options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
            context = new VisionDbContext(options);
            sut = new DockerVersionProvider(context, new DataProtectionStub(), Substitute.For<ILogger<DockerVersionProvider>>());
        }        

        [Theory]
        [InlineData("sonatype/nexus3", "latest")]
        public async Task DockerV2Api(string package, string expected)
        {
            // arrange
            context.Registries.Add(new Registry { Endpoint = "https://registry-1.docker.io/v2/", IsEnabled = true, IsPublic = true, Kind = DependencyKind.Docker });
            context.SaveChanges();

            Dependency dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = DependencyKind.NuGet };

            // act
            DependencyVersion latest = await sut.GetLatestMetaDataAsync(dependency);

            // assert
            Assert.Equal(expected, latest.Version);
        }

        public void Dispose() => context?.Dispose();
    }
}
