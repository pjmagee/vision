namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Vision.Core;
    using Xunit;

    public class AssetServiceTests : IDisposable
    {
        private readonly AssetService sut;
        private readonly VisionDbContext context;

        public AssetServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(AssetServiceTests)).Options);
            sut = new AssetService(context, Substitute.For<IAggregateAssetExtractor>(), Substitute.For<IDependencyService>());
        }

        [Fact]
        public async void GetByAssetIdAsync()
        {
            // TODO
            IPaginatedList<AssetDto> assets = await sut.GetByAssetIdAsync(Guid.Empty, "", new EcosystemKind[] { }, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByDependencyIdAsync()
        {
            // TODO
            IPaginatedList<AssetDto> assets = await sut.GetByDependencyIdAsync(Guid.Empty, "", new EcosystemKind[] { }, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByFrameworkIdAsync()
        {
            // TODO
            IPaginatedList<AssetDto> assets = await sut.GetByEcosystemIdAsync(Guid.Empty, "", new EcosystemKind[] { }, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByRepositoryIdAsync()
        {
            // TODO
            IPaginatedList<AssetDto> assets = await sut.GetByRepositoryIdAsync(Guid.Empty, "", new EcosystemKind[] { }, dependents: true, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByVcsIdAsync()
        {
            // TODO
            IPaginatedList<AssetDto> assets = await sut.GetByVcsIdAsync(Guid.Empty, "", new EcosystemKind[] { }, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByVersionIdAsync()
        {
            // TODO
            IPaginatedList<AssetDto> assets = await sut.GetByVersionIdAsync(Guid.Empty, "", new EcosystemKind[] { }, pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetPublishedNamesByRepositoryIdAsync()
        {
            // TODO
            List<string> assets = await sut.GetPublishedNamesByRepositoryIdAsync(Guid.Empty);
        }

        [Fact]
        public async void GetByIdAsync()
        {
            // TODO
            AssetDto asset = await sut.GetByIdAsync(Guid.Empty);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
