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

    [ApiController]
    [Route("api/[controller]")]
    [ResponseCache(Duration = 30)]
    [Produces("application/json")]        
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AssetsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public AssetsController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{assetId}")]
        public async Task<AssetDto> GetAssetByIdAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            return new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = await context.AssetDependencies.CountAsync(x => x.AssetId == assetId),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId
            };
        }

        [HttpGet("{assetId}/dependencies")]
        public async Task<IEnumerable<AssetDependencyDto>> GetDependenciesByAssetIdAsync(Guid assetId)
        {
            return await context.AssetDependencies.Where(assetDependency => assetDependency.AssetId == assetId).Select(assetDependency => new AssetDependencyDto
            {
                Repository = assetDependency.Asset.Repository.WebUrl,
                Dependency = assetDependency.Dependency.Name,
                Version = assetDependency.DependencyVersion.Version,
                AssetId = assetDependency.AssetId,
                Asset = assetDependency.Asset.Path,
                DependencyId = assetDependency.DependencyId,
                DependencyVersionId = assetDependency.DependencyVersionId,
                IsLatest = assetDependency.DependencyVersion.IsLatest,
                RepositoryId = assetDependency.Asset.RepositoryId
            })
            .ToListAsync();
        }

        [HttpGet("{assetId}/frameworks")]
        public async Task<IEnumerable<AssetFrameworkDto>> GetFrameworksByAssetId(Guid assetId)
        {
            return await context.AssetFrameworks
                .Where(x => x.AssetId == assetId)
                .Select(x => new AssetFrameworkDto { FrameworkId = x.FrameworkId, Name = x.Framework.Version, AssetId = x.AssetId })
                .ToListAsync();
        }        
    }
}
