using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using Microsoft.EntityFrameworkCore;

namespace Vision.Web.Core
{
    public class FakeProvider : ICICDProvider
    {

        private readonly VisionDbContext context;

        public FakeProvider(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);
            List<string> assets = await context.Assets.Where(x => x.RepositoryId == repositoryId).Select(x => x.Path).ToListAsync();
            assets.ForEach(asset => asset = Path.GetFileNameWithoutExtension(asset));

            string name = Pick<string>.RandomItemFrom(new[] { "Commit Build", "Feature Tests", "Integration Tests" });

            return Enumerable.Range(0, GetRandom.Int(1, 4)).Select(x => new CiCdBuildDto { Name = name, WebUrl = "http://ci.rbxd.ds:8090/path/to/build/" }).ToList();
        }

        public bool Supports(CiCdKind Kind) => true;
    }
}
