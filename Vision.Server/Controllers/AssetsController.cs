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

        [HttpGet("{repositoryId}")]
        public async Task<IEnumerable<AssetDto>> GetAsync(Guid repositoryId)
        {
            IEnumerable<Asset> assets = await context.Assets.Where(x => x.GitRepositoryId == repositoryId).ToListAsync();
            return assets.Select(a => new AssetDto { AssetId = a.Id, Dependencies = a.Dependencies.Count, Path = a.Path, RepositoryId = a.GitRepositoryId });
        }

        [HttpGet("{assetId}/dependencies")]
        public async Task<IEnumerable<AssetDependencyDto>> GetByAssetId(Guid assetId)
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
