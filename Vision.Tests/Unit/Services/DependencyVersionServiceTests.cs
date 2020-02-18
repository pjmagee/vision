namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using Vision.Core;
    using Xunit;

    public class DependencyVersionServiceTests : IDisposable
    {
        private readonly DependencyVersionService sut;
        private readonly VisionDbContext context;

        public DependencyVersionServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(CiCdServiceTests)).Options);
            sut = new DependencyVersionService(context);
        }

        [Fact]
        public async void GetByIdAsync()
        {
            // TODO
            DependencyVersionDto dependencyVersion = await sut.GetByIdAsync(Guid.Empty);
        }

        [Fact]
        public async void GetByDependencyIdAsync()
        {
            // TODO
            IPaginatedList<DependencyVersionDto> versions = await sut.GetByDependencyIdAsync(Guid.Empty, pageIndex: 1, pageSize: 1);
        }

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
