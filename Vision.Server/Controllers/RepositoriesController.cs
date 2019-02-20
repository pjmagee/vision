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
    public class RepositoriesController : ControllerBase
    {
        private readonly VisionDbContext context;

        public RepositoriesController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{sourceId}")]
        public async Task<IEnumerable<RepositoryDto>> GetAsync(Guid sourceId)
        {
            var repositories = await context.GitRepositories.Where(x => x.GitSourceId == sourceId).ToListAsync();
            return repositories.Select(x => new RepositoryDto { SourceId = x.GitSourceId, Assets = x.Assets.Count, GitUrl = x.GitUrl, WebUrl = x.WebUrl, RepositoryId = x.Id });
        }
    }
}
