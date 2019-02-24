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
    public class AssetsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public AssetsController(VisionDbContext context)
        {
            this.context = context;
        }
        
        [HttpGet("{id}")]
        public async Task<AssetDto> GetAssetByIdAsync(Guid id)
        {
            Asset asset = await context.Assets.FindAsync(id);

            return new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = await context.AssetDependencies.CountAsync(x => x.AssetId == id),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId
            };
        }

        [HttpGet("{id}/dependencies")]
        public async Task<ActionResult<dynamic>> GetDependenciesByAssetIdAsync(Guid id)
        {
            return Ok(await context.AssetDependencies.Where(x => x.AssetId == id).Select(x => new
            {
                x.Asset.Repository.WebUrl,
                x.Dependency.Name,
                x.DependencyVersion.Version,
                x.AssetId,
                x.DependencyId,
                x.DependencyVersionId,
            })
            .ToListAsync());
        }

        [HttpGet("{id}/frameworks")]
        public async Task<IEnumerable<AssetFrameworkDto>> GetFrameworksByAssetId(Guid id)
        {
            return await context.AssetFrameworks
                .Where(x => x.AssetId == id)
                .Select(x => new AssetFrameworkDto { FrameworkId = x.FrameworkId, Name = x.Framework.Version, AssetId = x.AssetId })
                .ToListAsync();
        }

        [HttpPost("/search")]
        public async Task<IEnumerable<AssetDto>> GetAssetByIdAsync([FromBody] AssetSearch assetSearch)
        {
            var dependencyIds = assetSearch.DependencyIds;
            var versionControlIds = assetSearch.VersionControlIds;

            IQueryable<Asset> query = context.Assets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(assetSearch.Name))
            {
                query = query.Where(asset => asset.Path.Contains(assetSearch.Name));
            }

            if (dependencyIds.Any())
            {
                query = query.Where(asset => versionControlIds.Contains(asset.Repository.VersionControlId));
            }

            if (versionControlIds.Any())
            {
                query = query.Where(asset => context.AssetDependencies.Any(assetDependency => assetDependency.AssetId == asset.Id && dependencyIds.Contains(assetDependency.Id)));
            }

            return await query.Select(asset => new AssetDto { AssetId = asset.Id, Dependencies = context.AssetDependencies.Count(x => x.AssetId == x.Id), Asset = asset.Path, RepositoryId = asset.RepositoryId }).ToListAsync();
        }
    }
}
