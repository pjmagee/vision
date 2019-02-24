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
            return await context.VersionControls.Select(x => new VersionControlDto
            {
                Endpoint = x.Endpoint,
                ApiKey = x.ApiKey,
                Kind = x.Kind,
                VersionControlId = x.Id,
                Repositories = context.Repositories.Count(x => x.VersionControlId == x.Id)
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

        [HttpGet("{id}")]
        public async Task<VersionControlDto> GetVersionControlByIdAsync(Guid id)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(id);

            return new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VersionControlId = versionControl.Id,
                Repositories = await context.Repositories.CountAsync(x => x.VersionControlId == versionControl.Id)
            };
        }

        [HttpGet("{id}/repositories")]
        public async Task<IEnumerable<RepositoryDto>> GetRepositoriesByIdAsync(Guid id)
        {
            var repositories = await context.Repositories.Where(x => x.VersionControlId == id).ToListAsync();
            return repositories.Select(x => new RepositoryDto { VersionControlId = x.VersionControlId, Assets = x.Assets.Count, Url = x.Url, WebUrl = x.WebUrl, RepositoryId = x.Id });
        }

       
    }
}
