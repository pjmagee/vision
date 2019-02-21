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
    public class DependenciesController : ControllerBase
    {
        private readonly VisionDbContext context;

        public DependenciesController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<DependencyDto>> GetAllDependenciesAsync()
        {
            var dependencies = await context.Dependencies.ToListAsync();

            return await Task.WhenAll(dependencies.Select(async x => new DependencyDto
            {
                DependencyId = x.Id,
                Name = x.Name,
                Kind = x.Kind,
                Assets = await context.AssetDependencies.CountAsync(a => a.DependencyId == x.Id),
                Versions = await context.DependencyVersions.CountAsync(x => x.DependencyId == x.Id),
                RepositoryUrl = x.RepositoryUrl
            }).ToList());
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

            return assetDependencies.Select(x => new AssetDependencyDto
            {
                Repository = x.Asset.GitRepository.WebUrl,
                Name = x.Asset.Path,
                Version = x.DependencyVersion.Version,
                AssetId = x.AssetId,
                DependencyId = dependencyId,
                DependencyVersionId = x.DependencyVersionId,
            });
        }

        [HttpGet("{dependencyId}/versions")]
        public async Task<IEnumerable<DependencyVersionDto>> GetVersionsByDependencyIdAsync(Guid dependencyId)
        {
            var versions = await context.DependencyVersions.Where(x => x.DependencyId == dependencyId).ToListAsync();

            return versions.Select(x => new DependencyVersionDto { DependencyId = x.DependencyId, DependencyVersionId = x.Id, IsVulnerable = x.IsVulnerable, Version = x.Version, VulnerabilityUrl = x.VulnerabilityUrl });
        }
    }
}
