namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;
    using System.Threading.Tasks;
    using Vision.Web.Core;
    using Xunit;

    public class DependencyServiceTests : IDisposable
    {
        private readonly DependencyService sut;
        private readonly VisionDbContext context;
        private readonly LoggerFactory loggerFactory;

        public DependencyServiceTests()
        {
            loggerFactory = new LoggerFactory();
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("DependencyServiceTests").Options);
            sut = new DependencyService(context, Substitute.For<IAssetService>());
        }

        [Fact]
        public async Task GetByKindsAsyncTest()
        {
            // Arrange
            context.Dependencies.AddRange(new Dependency { Name = "1", Kind = DependencyKind.Docker }, new Dependency { Name = "2", Kind = DependencyKind.Docker });
            await context.SaveChangesAsync();

            // Act
            IPaginatedList<DependencyDto> result = await sut.GetByKindsAsync(new[] { DependencyKind.Docker });

            // Assert
            result[0].Name = "1";
            result[1].Name = "2";
        }

        [Fact]
        public async Task GetAsyncTest()
        {
            // Arrange
            context.Dependencies.AddRange(new Dependency { Name = "1", Kind = DependencyKind.Docker }, new Dependency { Name = "2", Kind = DependencyKind.Docker });
            await context.SaveChangesAsync();

            // Act
            IPaginatedList<DependencyDto> result = await sut.GetAsync();

            // Assert
            Assert.Equal(1, result.TotalPages);
            Assert.False(result.HasNextPage);
            Assert.False(result.HasPreviousPage);

            Assert.Equal("1", result[0].Name);
            Assert.Equal(DependencyKind.Docker, result[0].Kind);

            Assert.Equal("2", result[1].Name);
            Assert.Equal(DependencyKind.Docker, result[1].Kind);
        }

        public void Dispose()
        {
            loggerFactory?.Dispose();
            context?.Dispose();
        }
    }
}
