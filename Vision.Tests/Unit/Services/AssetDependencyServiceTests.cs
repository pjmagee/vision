namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using Vision.Web.Core;
    using Xunit;

    public class AssetDependencyServiceTests : IDisposable
    {
        private readonly AssetDependencyService sut;
        private readonly VisionDbContext context;

        public AssetDependencyServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(AssetServiceTests)).Options);
            sut = new AssetDependencyService(context);
        }

        [Fact]
        public async void GetByAssetIdAsync()
        {
            // TODO
            IPaginatedList<AssetDependencyDto> assets = await sut.GetByAssetIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByDependencyIdAsync()
        {
            // TODO
            IPaginatedList<AssetDependencyDto> assets = await sut.GetByDependencyIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
