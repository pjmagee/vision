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
    [ApiController, Route("api/[controller]"), Produces("application/json"), ApiConventionType(typeof(DefaultApiConventions))]
    public class SourcesController : ControllerBase
    {
        private readonly VisionDbContext context;

        public SourcesController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<GitSourceDto>> GetAllSourcesAsync()
        {
            var sources = await context.GitSources.ToListAsync();
            return sources.Select(x => new GitSourceDto { Endpoint = x.Endpoint, ApiKey = x.ApiKey, Kind = x.Kind, SourceId = x.Id, Repositories = x.GitRepositories.Count });
        }

        [HttpGet("{sourceId}")]
        public async Task<GitSourceDto> GetSourceByIdAsync(Guid sourceId)
        {
            GitSource source = await context.GitSources.FindAsync(sourceId);
            return new GitSourceDto { Endpoint = source.Endpoint, ApiKey = source.ApiKey, Kind = source.Kind, SourceId = source.Id, Repositories = source.GitRepositories.Count };
        }

        [HttpGet("{sourceId}/repositories")]
        public async Task<IEnumerable<RepositoryDto>> GetRepositoriesBySourceIdAsync(Guid sourceId)
        {
            var repositories = await context.GitRepositories.Where(x => x.GitSourceId == sourceId).ToListAsync();
            return repositories.Select(x => new RepositoryDto { SourceId = x.GitSourceId, Assets = x.Assets.Count, GitUrl = x.GitUrl, WebUrl = x.WebUrl, RepositoryId = x.Id });
        }

        [HttpPost]
        [ProducesResponseType(400)]
        public async Task<ActionResult<GitSourceDto>> CreateGitSource([FromBody] GitSourceDto post)
        {
            var source = new GitSource { Id = Guid.NewGuid(), ApiKey = post.ApiKey, Endpoint = post.Endpoint, Kind = post.Kind };

            context.GitSources.Add(source);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateGitSource), new { sourceId = source.Id }, new GitSourceDto { Endpoint = source.Endpoint, ApiKey = source.ApiKey, Kind = source.Kind, SourceId = source.Id, Repositories = source.GitRepositories.Count });
        }
    }
}
