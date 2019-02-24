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
                Assets = await context.Assets.CountAsync(a => a.RepositoryId == repositoryId),
                Url = repository.Url,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id
            };
        }

        [HttpGet("{repositoryId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetByRepositoryId(Guid repositoryId)
        {
            return await context.Assets.Where(x => x.RepositoryId == repositoryId).Select(asset => new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId
            })
            .ToListAsync();
        }

        [HttpGet("{repositoryId}/frameworks")]
        public async Task<IEnumerable<AssetFrameworkDto>> GetFrameworksByRepositoryId(Guid repositoryId)
        {
            return await context.Assets
                .Where(asset => asset.RepositoryId == repositoryId) // assets in this repository
                .SelectMany(asset => context.AssetFrameworks.Where(x => x.AssetId == asset.Id)) // all asset frameworks that this asset uses
                .Select(x => x.Framework).Distinct() // select the linked unique frameworks and distinct them
                .Select(framework => new AssetFrameworkDto
                {
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