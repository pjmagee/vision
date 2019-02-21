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
    public class AssetsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public AssetsController(VisionDbContext context)
        {
            this.context = context;
        }
        
        /// <summary>
        /// Information about the asset.
        /// </summary>
        /// <param name="assetId">The Asset Id</param>
        /// <returns>The asset details</returns>
        [HttpGet("{assetId}")]
        public async Task<AssetDto> GetAssetByIdAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            return new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = await context.AssetDependencies.CountAsync(x => x.AssetId == assetId),
                Path = asset.Path,
                RepositoryId = asset.GitRepositoryId
            };
        }

        [HttpGet("{assetId}/dependencies")]
        public async Task<IEnumerable<AssetDependencyDto>> GetDependenciesByAssetIdAsync(Guid assetId)
        {
            IEnumerable<AssetDependency> assetDependencies = await context.AssetDependencies.Where(x => x.AssetId == assetId).ToListAsync();

            return assetDependencies.Select(x => new AssetDependencyDto
            {
                Repository = x.Asset.GitRepository.WebUrl,
                Name = x.Dependency.Name,
                Version = x.DependencyVersion.Version,
                AssetId = x.AssetId,
                DependencyId = x.DependencyId,
                DependencyVersionId = x.DependencyVersionId,
            });
        }

       
    }
}
