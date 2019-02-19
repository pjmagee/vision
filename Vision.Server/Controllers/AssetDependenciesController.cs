using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vision.Core;
using Vision.Shared.Api;

namespace Vision.Server.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AssetDependenciesController : ControllerBase
    {
        private readonly IAssetDependencyRepository assetDependencyRepository;

        public AssetDependenciesController(IAssetDependencyRepository assetDependencyRepository)
        {
            this.assetDependencyRepository = assetDependencyRepository;
        }

        [HttpGet("{assetId}")]
        public async Task<IEnumerable<AssetDependencyDto>> Get(Guid assetId)
        {
            IEnumerable<AssetDependency> assetDependencies = await assetDependencyRepository.GetByAssetIdAsync(assetId);

            return assetDependencies.Select(x => new AssetDependencyDto
            {
                Name = x.DependencyVersion.Dependency.Name,
                Version = x.DependencyVersion.Version,
                AssetId = x.AssetId,
                DependencyId = x.DependencyVersion.DependencyId,
                DependencyVersionId = x.DependencyVersionId,
            });
        }
    }
}
