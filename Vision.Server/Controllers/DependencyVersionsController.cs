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
    public class DependencyVersionsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public DependencyVersionsController(VisionDbContext context) => this.context = context;

        [HttpGet("{dependencyVersionId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetAssetsByDependencyVersionIdAsync(Guid dependencyVersionId)
        {
            return await context.Assets.Where(asset => context.AssetDependencies.Any(ad => ad.AssetId == asset.Id && ad.DependencyVersionId == dependencyVersionId)).Select(asset => new AssetDto
            {                
                AssetId = asset.Id,
                Asset = asset.Path,                          
                Dependencies = context.AssetDependencies.Count(a => a.AssetId == asset.Id),
                RepositoryId = asset.RepositoryId
            })
            .ToListAsync();
        }
    }
}
