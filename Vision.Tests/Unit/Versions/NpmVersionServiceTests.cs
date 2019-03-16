﻿namespace Vision.Tests
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
        [InlineData("https://registry.npmjs.org/", "@angular/core", "7.2.9")]
        public async Task NpmApiTest(string endpoint, string package, string expected)
        {
            // arrange
            context.Registries.Add(new Registry { Endpoint = endpoint, IsEnabled = true, IsPublic = true, Kind = DependencyKind.Npm });
            context.SaveChanges();

            Dependency dependency = new Dependency { Name = package, Kind = DependencyKind.Npm };

            // act
            DependencyVersion latest = await sut.GetLatestMetaDataAsync(dependency);

            // assert
            Assert.Equal(expected, latest.Version);
        }

        public void Dispose() => context?.Dispose();
    }
}
