using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;

namespace Vision.Web.Core
{
    public class FakeProvider : ICiCdProvider
    {
        private static string[] FakeBuilds = new string[]
        {
            "Commit Build",
            "Docker Build",
            "Feature Tests",
            "Integration Tests",
            "Code Quality Scan",
            "Security Scan",
            "Deploy to DEV",
            "Deploy to SYSTEST",
            "Deploy to UAT",
            "Deploy to OAT",
            "Deploy to LIVE",
        };

        public Task<List<CiCdBuildDto>> GetBuildsAsync(RepositoryDto repository, CiCdDto cicd)
        {
            var picker = new UniqueRandomPicker<string>(With.Between(1).And(FakeBuilds.Length).Elements, new UniqueRandomGenerator());
            var items = picker.From(FakeBuilds);            
            return Task.FromResult(items.Select(x => new CiCdBuildDto { Name = x, WebUrl = $"http://ci.rbxd.ds:8090/path/to/build/{x}" }).ToList());
        }

        public bool Supports(CiCdKind Kind) => true;
    }
}
