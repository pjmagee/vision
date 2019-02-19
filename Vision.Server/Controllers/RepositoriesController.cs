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
    public class RepositoriesController : ControllerBase
    {
        private readonly IGitRepositoryRepository gitRepositoryRepository;

        public RepositoriesController(IGitRepositoryRepository gitRepositoryRepository)
        {
            this.gitRepositoryRepository = gitRepositoryRepository;
        }

        [HttpGet("{sourceId}")]
        public async Task<IEnumerable<RepositoryDto>> GetAsync(Guid sourceId)
        {
            var repositories = await gitRepositoryRepository.GetBySourceIdAsync(sourceId);
            return repositories.Select(x => new RepositoryDto { Assets = x.Assets.Count, GitUrl = x.GitUrl, WebUrl = x.WebUrl, RepositoryId = x.Id });
        }
    }
}
