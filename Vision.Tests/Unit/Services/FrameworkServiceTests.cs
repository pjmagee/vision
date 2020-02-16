namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using Vision.Web.Core;
    using Xunit;

    public class FrameworkServiceTests : IDisposable
    {
        private readonly RuntimeService sut;
        private readonly VisionDbContext context;

        public FrameworkServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(FrameworkServiceTests)).Options);
            sut = new RuntimeService(context);
        }

        [Fact]
        public async void GetAsync()
        {
            // TODO
            IPaginatedList<RuntimeDto> frameworks = await sut.GetAsync(pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByAssetIdAsync()
        {
            // TODO
            IPaginatedList<RuntimeDto> frameworks = await sut.GetByAssetIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByRepositoryIdAsync()
        {
            // TODO
            IPaginatedList<RuntimeDto> frameworks = await sut.GetByRepositoryIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByIdAsync()
        {
            // TODO
            RuntimeDto framework = await sut.GetRuntimeByIdAsync(Guid.Empty);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
