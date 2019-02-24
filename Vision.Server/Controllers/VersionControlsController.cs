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
    public class VersionControlsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public VersionControlsController(VisionDbContext context) => this.context = context;

        [HttpGet]
        public async Task<IEnumerable<VersionControlDto>> GetAllAsync()
        {
            return await context.VersionControls.Select(versionControl => new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VersionControlId = versionControl.Id,
                Repositories = context.Repositories.Count(repository => repository.VersionControlId == versionControl.Id)
            }).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<VersionControlDto>> PostVersionControlAsync([FromBody] VersionControlDto post)
        {
            VersionControl versionControl = new VersionControl { Id = Guid.NewGuid(), ApiKey = post.ApiKey, Endpoint = post.Endpoint, Kind = post.Kind };

            context.VersionControls.Add(versionControl);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostVersionControlAsync), new { sourceId = versionControl.Id }, new VersionControlDto { Endpoint = versionControl.Endpoint, ApiKey = versionControl.ApiKey, Kind = versionControl.Kind, VersionControlId = versionControl.Id, Repositories = versionControl.Repositories.Count });
        }

        [HttpGet("{versionControlId}")]
        public async Task<VersionControlDto> GetVersionControlByIdAsync(Guid versionControlId)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

            return new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VersionControlId = versionControl.Id,
                Repositories = await context.Repositories.CountAsync(x => x.VersionControlId == versionControl.Id)
            };
        }

        [HttpGet("{versionControlId}/metrics")]
        public async Task<IEnumerable<MetricDto>> GetMetricsForVersionControlById(Guid versionControlId)
        {
            // top 5 largest repositories
            // top 5 smallest repositories

            // top 5 largest assets
            // top 5 smallest assets

            // top 5 assets with most updated dependencies
            // top 5 assets with most outdated dependencies
        }

        [HttpGet("{versionControlId}/repositories")]
        public async Task<IEnumerable<RepositoryDto>> GetRepositoriesByIdAsync(Guid versionControlId)
        {
            var repositories = await context.Repositories.Where(repository => repository.VersionControlId == versionControlId).ToListAsync();
            return repositories.Select(x => new RepositoryDto { VersionControlId = x.VersionControlId, Assets = x.Assets.Count, Url = x.Url, WebUrl = x.WebUrl, RepositoryId = x.Id });
        }

        [HttpGet("{versionControlId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetAssetsByVersionControlIdAsync(Guid versionControlId)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

            return await context.Assets
                    .Where(asset => context.Repositories.Any(repository => repository.VersionControlId == versionControlId && asset.RepositoryId == repository.Id))
                    .Select(asset => new AssetDto
                    {
                        Asset = asset.Path,
                        AssetId = asset.Id,
                        Repository = asset.Repository.Url,
                        RepositoryId = asset.RepositoryId
                    })
                    .ToListAsync();
        }


    }
}
