using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ApiController, Route("api/[controller]"), Produces("application/json"), ApiConventionType(typeof(DefaultApiConventions))]
    public class RefreshController : ControllerBase
    {
        private readonly VisionDbContext context;

        public RefreshController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet("/tasks")]
        public async Task<IEnumerable<RefreshTask>> GetAllRefreshTasksAsync()
        {
            return await context.RefreshTasks.ToListAsync();
        }

        [HttpPost("asset/{assetId}")]
        public async Task<ActionResult<RefreshTask>> PostRefreshAssetAsync(Guid assetId)
        {
            RefreshTask task = new RefreshTask { Created = DateTime.Now, Id = Guid.NewGuid(), Kind = RefreshKind.Asset, Status = RefreshStatus.Pending, TargetId = assetId };
            context.RefreshTasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostRefreshAssetAsync), task);
        }

        [HttpPost("dependency/{dependencyId}")]
        public async Task<ActionResult<RefreshTask>> PostRefreshDependencyAsync(Guid dependencyId)
        {
            RefreshTask task = new RefreshTask { Id = Guid.NewGuid(), Kind = RefreshKind.Dependency, Status = RefreshStatus.Pending, TargetId = dependencyId };
            context.RefreshTasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostRefreshDependencyAsync), task);
        }

        [HttpPost("repository/{repositoryId}")]
        public async Task<ActionResult<RefreshTask>> PostRefreshGitRepositoryAsync(Guid repositoryId)
        {
            RefreshTask task = new RefreshTask { Id = Guid.NewGuid(), Kind = RefreshKind.GitRepository, Status = RefreshStatus.Pending, TargetId = repositoryId };
            context.RefreshTasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostRefreshGitRepositoryAsync), task);
        }

        [HttpPost("source/{sourceId}")]
        public async Task<ActionResult<RefreshTask>> PostRefreshGitServerAsync(Guid sourceId)
        {
            RefreshTask task = new RefreshTask { Id = Guid.NewGuid(), Kind = RefreshKind.GitServer, Status = RefreshStatus.Pending, TargetId = sourceId };
            context.RefreshTasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostRefreshGitServerAsync), task);
        }
    }
}
