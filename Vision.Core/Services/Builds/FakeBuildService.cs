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
    public class FakeBuildService : IBuildService
    {
        private readonly VisionDbContext context;

        public FakeBuildService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Build>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            await Task.Delay(2000); // simulate network latency

            GitRepository repository = await context.GitRepositories.FindAsync(repositoryId);
            List<string> assets = await context.Assets.Where(x => x.GitRepositoryId == repositoryId).Select(x => x.Path).ToListAsync();
            assets.ForEach(asset => asset = Path.GetFileNameWithoutExtension(asset));

            string name = Pick<string>.RandomItemFrom(new[] { "Commit Build", "Feature Tests", "Integration Tests" });

            return Enumerable.Range(0, GetRandom.Int(1, 3)).Select(x => new Build { Name = name, WebUrl = "http://ci.rbxd.ds:8090/path/to/build/" });
        }
    }
}
