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

        public FrameworksController(VisionDbContext context) => this.context = context;

        [HttpGet]
        public async Task<IEnumerable<FrameworkDto>> GetAllAsync()
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
                Assets = await context.AssetFrameworks.CountAsync(assetFramework => assetFramework.FrameworkId == frameworkId)
            };
        }

        [HttpGet("{frameworkId}/repositories")]
        public async Task<IEnumerable<RepositoryDto>> GetRepositoriesByFrameworkIdAsync(Guid frameworkId)
        {
            var framework = await context.Frameworks.FindAsync(frameworkId);

            return await context.Repositories
                     .Where(repository => context.Assets.Any(asset => asset.RepositoryId == repository.Id && context.AssetFrameworks.Any(af => af.FrameworkId == frameworkId && af.AssetId == asset.Id)))
                     .Select(repository => new RepositoryDto
                     {
                         Assets = context.Assets.Count(asset => asset.RepositoryId == repository.Id),
                         VersionControlId = repository.VersionControlId,
                         WebUrl = repository.WebUrl,
                         Url = repository.Url,
                         RepositoryId = repository.Id
                     })
                     .ToListAsync();
        }

        [HttpGet("{frameworkId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetAssetsByFrameworkIdAsync(Guid frameworkId)
        {
            List<Guid> assetIds = await context.AssetFrameworks.Where(assetFramework => assetFramework.FrameworkId == frameworkId).Select(x => x.AssetId).ToListAsync();

            return await context.Assets
                .Where(asset => assetIds.Contains(asset.Id))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Asset = asset.Path,
                    Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                    Repository = asset.Repository.WebUrl,
                    RepositoryId = asset.RepositoryId
                }).ToListAsync();
        }
    }
}
