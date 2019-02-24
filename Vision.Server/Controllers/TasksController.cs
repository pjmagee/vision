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
    public class TasksController : ControllerBase
    {
        private readonly VisionDbContext context;

        public TasksController(VisionDbContext context) => this.context = context;

        [HttpGet]
        public async Task<IEnumerable<SystemTask>> GetAllAsync() => await context.Tasks.ToListAsync();

        [HttpPost("update/asset/{id}")]
        public async Task<ActionResult<SystemTask>> PostUpdateAssetAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScope.Asset, Status = Shared.TaskStatus.Pending, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostUpdateAssetAsync), task);
        }

        [HttpPost("update/dependency/{id}")]
        public async Task<ActionResult<SystemTask>> PostUpdateDependencyAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScope.Dependency, Status = Shared.TaskStatus.Pending, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostUpdateDependencyAsync), task);
        }

        [HttpPost("update/repository/{id}")]
        public async Task<ActionResult<SystemTask>> PostUpdateRepositoryAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScope.Repository, Status = Shared.TaskStatus.Pending, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostUpdateRepositoryAsync), task);
        }

        [HttpPost("update/versioncontrol/{id}")]
        public async Task<ActionResult<SystemTask>> PostUpdateVersionControlAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScope.VersionControl, Status = Shared.TaskStatus.Pending, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostUpdateVersionControlAsync), task);
        }
    }
}
