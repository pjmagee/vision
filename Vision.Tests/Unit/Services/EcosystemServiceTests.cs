namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using Vision.Core;
    using Xunit;

    public class EcosystemServiceTests : IDisposable
    {
        private readonly EcosystemService sut;
        private readonly VisionDbContext context;

        public EcosystemServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(EcosystemServiceTests)).Options);
            sut = new EcosystemService(context);
        }

        [Fact]
        public async void GetAsync()
        {
            // TODO
            IPaginatedList<EcosystemDto> ecosystems = await sut.GetAsync(pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByAssetIdAsync()
        {
            // TODO
            IPaginatedList<EcosystemDto> ecosystems = await sut.GetByAssetIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByRepositoryIdAsync()
        {
            // TODO
            IPaginatedList<EcosystemDto> ecosystems = await sut.GetByRepositoryIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByIdAsync()
        {
            // TODO
            EcosystemDto ecosystem = await sut.GetEcosystemByIdAsync(Guid.Empty);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
