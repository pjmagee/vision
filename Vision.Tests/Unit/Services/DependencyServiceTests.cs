namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Vision.Core;
    using Xunit;

    public class DependencyServiceTests : IDisposable
    {
        private readonly DependencyService sut;
        private readonly VisionDbContext context;

        public DependencyServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(DependencyServiceTests)).Options);
            sut = new DependencyService(context, Substitute.For<IAggregateAssetExtractor>());
        }

        [Fact]
        public async Task GetAsync()
        {
            // Arrange
            context.Dependencies.AddRange(new Dependency { Name = "1", Kind = EcosystemKind.Docker }, new Dependency { Name = "2", Kind = EcosystemKind.Docker });
            await context.SaveChangesAsync();

            // Act
            IPaginatedList<DependencyDto> result = await sut.GetAsync(Enumerable.Empty<EcosystemKind>(), null);

            // Assert
            Assert.Equal(1, result.TotalPages);
            Assert.False(result.HasNextPage);
            Assert.False(result.HasPreviousPage);

            Assert.Equal("1", result[0].Name);
            Assert.Equal(EcosystemKind.Docker, result[0].Kind);

            Assert.Equal("2", result[1].Name);
            Assert.Equal(EcosystemKind.Docker, result[1].Kind);
        }

        [Fact]
        public async Task GetByRepositoryIdAsync()
        {
            // TODO
            IPaginatedList<DependencyDto> result = await sut.GetByRepositoryIdAsync(Guid.Empty, Enumerable.Empty<EcosystemKind>(), search: "", pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async Task GetByAssetIdAsync()
        {
            // TODO
            IPaginatedList<DependencyDto> result = await sut.GetByAssetIdAsync(Guid.Empty, Enumerable.Empty<EcosystemKind>(), search: "", pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async Task GetByIdAsync()
        {
            // TODO
            DependencyDto result = await sut.GetByIdAsync(Guid.Empty);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
