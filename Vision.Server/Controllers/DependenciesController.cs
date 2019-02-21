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
    public class DependenciesController : ControllerBase
    {
        private readonly VisionDbContext context;

        public DependenciesController(VisionDbContext context) => this.context = context;


        [HttpGet]
        public async Task<IEnumerable<DependencyDto>> GetAllDependenciesAsync()
        {
            return await context.Dependencies.Select(dependency => new DependencyDto
            {
                DependencyId  = dependency.Id,
                Name = dependency.Name,
                Kind = dependency.Kind,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id),
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                RepositoryUrl = dependency.RepositoryUrl
            })
            .ToListAsync();
        }

        [HttpGet("{dependencyId}")]
        public async Task<DependencyDto> GetDependencyByIdAsync(Guid dependencyId)
        {
            var dependency = await context.Dependencies.FindAsync(dependencyId);

            return new DependencyDto
            {
                Name = dependency.Name,
                Kind = dependency.Kind,
                DependencyId = dependency.Id,
                RepositoryUrl = dependency.RepositoryUrl,
                Assets = await context.AssetDependencies.CountAsync(x => x.DependencyId == dependencyId),
                Versions = await context.DependencyVersions.CountAsync(x => x.DependencyId == dependencyId),
            };
        }

        [HttpGet("{dependencyId}/assets")]
        public async Task<IEnumerable<AssetDependencyDto>> GetAssetsByDependencyIdAsync(Guid dependencyId)
        {
            IEnumerable<AssetDependency> assetDependencies = await context.AssetDependencies.Where(x => x.DependencyId == dependencyId).ToListAsync();

            return assetDependencies.Select(assetDependency => new AssetDependencyDto
            {
                DependencyId = dependencyId,
                AssetId = assetDependency.AssetId,                
                Name = assetDependency.Asset.Path,
                Version = assetDependency.DependencyVersion.Version,
                DependencyVersionId = assetDependency.DependencyVersionId,
                Repository = assetDependency.Asset.GitRepository.WebUrl
            });
        }

        [HttpGet("{dependencyId}/versions")]
        public async Task<IEnumerable<DependencyVersionDto>> GetVersionsByDependencyIdAsync(Guid dependencyId)
        {
            var versions = await context.DependencyVersions.Where(x => x.DependencyId == dependencyId).ToListAsync();

            return versions.Select(x => new DependencyVersionDto { DependencyId = x.DependencyId, DependencyVersionId = x.Id, Version = x.Version, VulnerabilityUrl = x.VulnerabilityUrl });
        }
    }
}
