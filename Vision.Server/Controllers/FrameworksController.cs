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
            var assets = await context.AssetFrameworks.Where(x => x.FrameworkId == frameworkId).Select(x => x.Asset).ToListAsync();

            return await Task.WhenAll(assets.Distinct().Select(async asset => new AssetDto
            {
                Repository = asset.GitRepository.WebUrl,
                RepositoryId = asset.GitRepositoryId,
                AssetId = asset.Id,
                Path = asset.Path,
                Dependencies = await context.AssetDependencies.CountAsync(x => x.AssetId == asset.Id)
            }));
        }
    }
}
