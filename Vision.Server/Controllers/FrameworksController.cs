using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ResponseCache(Duration = 30)]
    [ApiController, Route("api/[controller]"), Produces("application/json"), ApiConventionType(typeof(DefaultApiConventions))]
    public class FrameworksController : ControllerBase
    {
        private readonly VisionDbContext context;

        public FrameworksController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<FrameworkDto>> GetAsync()
        {
            var frameworks = await context.Frameworks.ToListAsync();

            return await Task.WhenAll(frameworks.Select(async framework => new FrameworkDto
            {
                Name = framework.Version,
                FrameworkId = framework.Id,
                Assets = await context.AssetFrameworks.CountAsync(af => af.FrameworkId == framework.Id)
            }));
        }

        [HttpGet("{frameworkId}")]
        public async Task<FrameworkDto> GetFrameworkByIdAsync(Guid frameworkId)
        {
            var framework = await context.Frameworks.FindAsync(frameworkId);

            return new FrameworkDto
            {
                Name = framework.Version,
                FrameworkId = framework.Id,
                Assets = await context.AssetFrameworks.CountAsync(af => af.FrameworkId == framework.Id)
            };
        }

        [HttpGet("{frameworkId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetAssetsByFrameworkIdAsync(Guid frameworkId)
        {
            List<Guid> assetIds = await context.AssetFrameworks.Where(x => x.FrameworkId == frameworkId).Select(x => x.AssetId).ToListAsync();

            return await context.Assets
                .Where(asset => assetIds.Contains(asset.Id))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Path = asset.Path,
                    Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id),
                    Repository = asset.GitRepository.WebUrl,
                    RepositoryId = asset.GitRepositoryId
                }).ToListAsync();
        }
    }
}
