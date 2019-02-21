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
    [ApiController, Route("api/[controller]")]
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
            return new AssetDto { AssetId = asset.Id, Dependencies = asset.Dependencies.Count, Path = asset.Path, RepositoryId = asset.GitRepositoryId };
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
