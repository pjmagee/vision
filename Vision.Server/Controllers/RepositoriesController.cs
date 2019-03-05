using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Core;
using Vision.Core.Services.Builds;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ResponseCache(Duration = 30)]
    [ApiController, Route("api/[controller]"), Produces("application/json"), ApiConventionType(typeof(DefaultApiConventions))]
    public class RepositoriesController : ControllerBase
    {
        private readonly VisionDbContext context;
        private readonly ICiCdChecker cicdChecker;

        public RepositoriesController(VisionDbContext context, ICiCdChecker cicdChecker)
        {
            this.context = context;
            this.cicdChecker = cicdChecker;
        }

        [HttpGet("{repositoryId}")]
        public async Task<RepositoryDto> GetRepositoryByIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            return new RepositoryDto
            {                
                VersionControlId = repository.VersionControlId,
                Assets = await context.Assets.CountAsync(asset => asset.RepositoryId == repositoryId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id
            };
        }

        [HttpGet("{repositoryId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetByRepositoryId(Guid repositoryId)
        {
            return await context.Assets.Where(asset => asset.RepositoryId == repositoryId).Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Repository = asset.Repository.Url,
                Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId
            })
            .ToListAsync();
        }

        [HttpGet("{repositoryId}/dependencies")]
        public async Task<IEnumerable<DependencyDto>> GetDependenciesByRepositoryId(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            return await context.Dependencies.Where(dependency => string.Equals(dependency.RepositoryUrl, repository.Url) || string.Equals(dependency.RepositoryUrl, repository.WebUrl)).Select(dependency => new DependencyDto
            {
                Assets = context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id),
                DependencyId = dependency.Id,
                Kind = dependency.Kind,
                Name = dependency.Name,
                RepositoryUrl = dependency.RepositoryUrl,
                Versions = context.DependencyVersions.Count(dv => dv.DependencyId == dependency.Id)
            })
            .ToListAsync();
        }

        [HttpGet("{repositoryId}/dependents")]
        public async Task<IEnumerable<AssetDto>> GetDependentsByRepositoryId(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            return await context.Assets
                .Where(asset => context.AssetDependencies.Where(ad => ad.AssetId == asset.Id).Any(ad => context.Dependencies.Any(d => ad.DependencyId == d.Id && string.Equals(d.RepositoryUrl, repository.Url) || string.Equals(d.RepositoryUrl, repository.WebUrl))))
                .Select(asset => new AssetDto
                {
                    AssetId = asset.Id,
                    Repository = asset.Repository.Url,
                    Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id),
                    Asset = asset.Path,
                    RepositoryId = asset.RepositoryId
                })
                .ToListAsync();
        }

        [HttpGet("{repositoryId}/frameworks")]
        public async Task<IEnumerable<FrameworkDto>> GetFrameworksByRepositoryId(Guid repositoryId)
        {
            return await context.Assets
                .Where(asset => asset.RepositoryId == repositoryId) // assets in this repository
                .SelectMany(asset => context.AssetFrameworks.Where(x => x.AssetId == asset.Id)) // all asset frameworks that this asset uses
                .Select(assetFramework => assetFramework.Framework).Distinct() // select the linked unique frameworks and distinct them
                .Select(framework => new FrameworkDto
                {
                    Assets = context.AssetFrameworks.Count(assetFramework => assetFramework.FrameworkId == framework.Id),
                    FrameworkId = framework.Id,
                    Name = framework.Version                    
                })
                .ToListAsync();
        }

        [HttpGet("{repositoryId}/cicds")]
        public async Task<IEnumerable<CiCdBuildDto>> GetCiCdsByRepositoryId(Guid repositoryId)
        {
            return await cicdChecker.GetBuildsByRepositoryIdAsync(repositoryId);
        }
    }
}