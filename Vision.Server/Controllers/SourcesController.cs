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
    public class GitSources : ControllerBase
    {
        private readonly IGitSourceRepository gitSourceRepository;

        public GitSources(IGitSourceRepository gitSourceRepository)
        {
            this.gitSourceRepository = gitSourceRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<GitSourceDto>> GetAsync()
        {
            var sources = await gitSourceRepository.GetAllAsync();
            return sources.Select(x => new GitSourceDto { Endpoint = x.Endpoint, ApiKey = x.ApiKey, Kind = x.Kind, SourceId = x.Id, Repositories = x.Repositories.Count });
        }

        [HttpGet("{sourceId}")]
        public async Task<GitSourceDto> Get(Guid sourceId)
        {
            GitSource source = await gitSourceRepository.GetByIdAsync(sourceId);
            return new GitSourceDto { Endpoint = source.Endpoint, ApiKey = source.ApiKey, Kind = source.Kind, SourceId = source.Id, Repositories = source.Repositories.Count };
        }

        [HttpPost]
        [ProducesResponseType(400)]
        public async Task<ActionResult<GitSourceDto>> PostAsync([FromBody] GitSourceDto post)
        {
            var source = new GitSource { Id = Guid.NewGuid(), ApiKey = post.ApiKey, Endpoint = post.Endpoint, Kind = post.Kind };

            await gitSourceRepository.SaveAsync(source);

            return CreatedAtAction(nameof(Get), new { sourceId = source.Id }, new GitSourceDto { Endpoint = source.Endpoint, ApiKey = source.ApiKey, Kind = source.Kind, SourceId = source.Id, Repositories = source.Repositories.Count });
        }

        [HttpPost("/refresh")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> RefreshAsync(Guid sourceId)
        {
            return Ok();
        }
    }
}
