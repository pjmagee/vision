﻿using System;
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
        public async Task<IEnumerable<DependencyDto>> GetAllAsync()
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

        [HttpPost]
        public async Task<IEnumerable<DependencyDto>> GetAllByKindsAsync([FromBody] DependencyKind[] kinds)
        {
            return await context.Dependencies.Where(dependency => kinds.Contains(dependency.Kind)).Select(dependency => new DependencyDto
            {
                DependencyId = dependency.Id,
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
            Dependency dependency = await context.Dependencies.FindAsync(dependencyId);

            return new DependencyDto
            {
                Name = dependency.Name,
                Kind = dependency.Kind,
                DependencyId = dependency.Id,
                RepositoryUrl = dependency.RepositoryUrl,
                Assets = await context.AssetDependencies.CountAsync(assetDependency => assetDependency.DependencyId == dependencyId),
                Versions = await context.DependencyVersions.CountAsync(dependencyVersion => dependencyVersion.DependencyId == dependencyId),
            };
        }


        [HttpGet("{dependencyId}/assets")]
        public async Task<IEnumerable<AssetDependencyDto>> GetAssetsByDependencyIdAsync(Guid dependencyId)
        {
            return await context.AssetDependencies.Where(ad => ad.DependencyId == dependencyId).Select(assetDependency => new AssetDependencyDto
            {   
                AssetId = assetDependency.AssetId,
                Version = assetDependency.DependencyVersion.Version,
                DependencyVersionId = assetDependency.DependencyVersionId,
                DependencyId = assetDependency.DependencyId,
                Repository = assetDependency.Asset.Repository.WebUrl,
                Asset = assetDependency.Asset.Path,
                IsLatest = assetDependency.DependencyVersion.IsLatest,
                Dependency = assetDependency.Dependency.Name,
                RepositoryId = assetDependency.Asset.RepositoryId                
            })
            .ToListAsync();
        }

        [HttpGet("{dependencyId}/versions")]
        public async Task<IEnumerable<DependencyVersionDto>> GetVersionsByDependencyIdAsync(Guid dependencyId)
        {
            return await context.DependencyVersions
                .Where(dependencyVersion => dependencyVersion.DependencyId == dependencyId)
                .Select(dependencyVersion => new DependencyVersionDto
                {
                    Assets = context.AssetDependencies.Count(assetDependency => assetDependency.DependencyVersionId == dependencyVersion.Id),
                    IsLatest = dependencyVersion.IsLatest,                    
                    DependencyId = dependencyVersion.DependencyId,
                    DependencyVersionId = dependencyVersion.Id,
                    Version = dependencyVersion.Version
                }).ToListAsync();
        }
    }
}
