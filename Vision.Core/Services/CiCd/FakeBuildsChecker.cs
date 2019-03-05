using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using Microsoft.EntityFrameworkCore;
using Vision.Shared;

namespace Vision.Core.Services.Builds
{
    public class FakeBuildsChecker : ICiCdChecker
    {
        private readonly VisionDbContext context;

        public FakeBuildsChecker(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);
            List<string> assets = await context.Assets.Where(x => x.RepositoryId == repositoryId).Select(x => x.Path).ToListAsync();
            assets.ForEach(asset => asset = Path.GetFileNameWithoutExtension(asset));

            string name = Pick<string>.RandomItemFrom(new[] { "Commit Build", "Feature Tests", "Integration Tests" });

            return Enumerable.Range(0, GetRandom.Int(1, 3)).Select(x => new CiCdBuildDto { Name = name, WebUrl = "http://ci.rbxd.ds:8090/path/to/build/" });
        }
    }
}
