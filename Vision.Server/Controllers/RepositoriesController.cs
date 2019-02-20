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

        [HttpGet("{repositoryId}/assets")]
        public async Task<IEnumerable<AssetDto>> GetByRepositoryId(Guid repositoryId)
        {
            IEnumerable<Asset> assets = await context.Assets.Where(x => x.GitRepositoryId == repositoryId).ToListAsync();
            return assets.Select(a => new AssetDto { AssetId = a.Id, Dependencies = a.Dependencies.Count, Path = a.Path, RepositoryId = a.GitRepositoryId });
        }

        [HttpGet("{repositoryId}")]
        public async Task<RepositoryDto> GetRepositoryByIdAsync(Guid repositoryId)
        {
            var repository = await context.GitRepositories.FindAsync(repositoryId);

            return new RepositoryDto
            {
                SourceId = repository.GitSourceId,
                Assets = await context.Assets.CountAsync(a => a.GitRepositoryId == repositoryId),
                GitUrl = repository.GitUrl,
                WebUrl = repository.WebUrl,
                RepositoryId = repository.Id
            };
        }
    }
}