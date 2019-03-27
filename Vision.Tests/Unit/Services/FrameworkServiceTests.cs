namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using Vision.Web.Core;
    using Xunit;

    public class FrameworkServiceTests : IDisposable
    {
        private readonly FrameworkService sut;
        private readonly VisionDbContext context;

        public FrameworkServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(FrameworkServiceTests)).Options);
            sut = new FrameworkService(context);
        }

        [Fact]
        public async void GetAsync()
        {
            // TODO
            IPaginatedList<FrameworkDto> frameworks = await sut.GetAsync(pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByAssetIdAsync()
        {
            // TODO
            IPaginatedList<FrameworkDto> frameworks = await sut.GetByAssetIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByRepositoryIdAsync()
        {
            // TODO
            IPaginatedList<FrameworkDto> frameworks = await sut.GetByRepositoryIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByIdAsync()
        {
            // TODO
            FrameworkDto framework = await sut.GetByIdAsync(Guid.Empty);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
