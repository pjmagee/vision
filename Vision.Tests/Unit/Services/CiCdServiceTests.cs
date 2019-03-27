namespace Vision.Tests
{
    using Microsoft.EntityFrameworkCore;
    using NSubstitute;
    using System;
    using Vision.Web.Core;
    using Xunit;

    public class CiCdServiceTests : IDisposable
    {
        private readonly CiCdService sut;
        private readonly VisionDbContext context;

        public CiCdServiceTests()
        {
            context = new VisionDbContext(new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase(nameof(CiCdServiceTests)).Options);
            sut = new CiCdService(context, Substitute.For<IEncryptionService>());
        }

        [Fact]
        public async void GetAsync()
        {
            // TODO
            IPaginatedList<CiCdDto> cicds = await sut.GetAsync(pageIndex: 1, pageSize: 1);
        }

        [Fact]
        public async void GetByIdAsync()
        {
            // TODO
            CiCdDto cicd = await sut.GetByIdAsync(Guid.Empty);
        }        

        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
