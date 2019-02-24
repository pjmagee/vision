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