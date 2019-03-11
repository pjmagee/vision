using Microsoft.AspNetCore.DataProtection;

namespace Vision.Tests
{

    public class DataProtectionStub : IDataProtectionProvider, IDataProtector
    {
        public DataProtectionStub()
        {
        }

        public IDataProtector CreateProtector(string purpose)
        {
            return new DataProtectionStub();
        }                

        public byte[] Protect(byte[] plaintext)
        {
            return plaintext;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }

    //public class NuGetVersionServiceTests
    //{
    //    private readonly NuGetVersionService service;

    //    private readonly DbContextOptions<VisionDbContext> options;

    //    private readonly VisionDbContext context;

    //    public NuGetVersionServiceTests()
    //    {
    //        options = new DbContextOptionsBuilder<VisionDbContext>().UseInMemoryDatabase("Registries").Options;
    //        context = new VisionDbContext(options);
    //        service = new NuGetVersionService(context, new DataProtectionStub(),  new LoggerFactory().CreateLogger<NuGetVersionService>());
    //    }

    //    [Theory]
    //    [InlineData("Atlassian.Stash.Api", "3.1.20" )]        
    //    public async Task NuGetV2Api(string package, string version)
    //    {
    //        // arrange
    //        context.Registries.Add(new Registry { Endpoint = "https://www.nuget.org/api/v2", ApiKey = "", IsEnabled = true, IsPublic = true, Kind = DependencyKind.NuGet });
    //        context.SaveChanges();
           
    //        var dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = DependencyKind.NuGet };

    //        // act
    //        var latest = await service.GetLatestVersionAsync(dependency);

    //        // assert
    //        Assert.Equal(version, latest.Version);
    //    }

    //    [Theory]
    //    [InlineData("Atlassian.Stash.Api", "3.1.20")]
    //    public async Task NuGetV3Api(string package, string version)
    //    {
    //        // arrange
    //        context.Registries.Add(new Registry { Endpoint = "https://api.nuget.org/v3/index.json", ApiKey = "", IsEnabled = true, IsPublic = true, Kind = DependencyKind.NuGet });
    //        context.SaveChanges();

    //        var dependency = new Dependency { Name = package, Id = Guid.NewGuid(), Kind = DependencyKind.NuGet };

    //        // act
    //        var latest = await service.GetLatestVersionAsync(dependency);

    //        // assert
    //        Assert.Equal(version, latest.Version);
    //    }
    //}
}
