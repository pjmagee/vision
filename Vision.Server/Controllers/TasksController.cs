using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vision.Core;
using Vision.Server.Hubs;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ApiController, Route("api/[controller]"), Produces("application/json"), ApiConventionType(typeof(DefaultApiConventions))]
    public class TasksController : ControllerBase
    {
        private readonly VisionDbContext context;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly ILogger<TasksController> logger;

        public TasksController(VisionDbContext context, IHubContext<NotificationHub> hubContext, ILogger<TasksController> logger)
        {
            this.context = context;
            this.hubContext = hubContext;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<SystemTask>> GetAllAsync() => await context.Tasks.ToListAsync();

        [HttpGet("update/assets/{id}")]
        public async Task<ActionResult<SystemTask>> UpdateAssetAsync(Guid id)
        {           
            try
            {
                await hubContext.Clients.All.SendAsync("Send", new AlertDto { Message = $"Refresh {id} task has been added.", Kind = MetricAlertKind.Standard });

                SystemTask task = new SystemTask { Scope = TaskScope.Asset, Status = Shared.TaskStatus.Pending, TargetId = id };
                context.Tasks.Add(task);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(UpdateAssetAsync), task);
            }
            catch(Exception e)
            {
                logger.LogError(e, "Error sending alert");
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("update/dependencies/{id}")]
        public async Task<ActionResult<SystemTask>> UpdateDependencyAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScope.Dependency, Status = Shared.TaskStatus.Pending, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(UpdateDependencyAsync), task);
        }

        [HttpGet("update/repositories/{id}")]
        public async Task<ActionResult<SystemTask>> UpdateRepositoryAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScope.Repository, Status = Shared.TaskStatus.Pending, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(UpdateRepositoryAsync), task);
        }

        [HttpGet("update/vcs/{id}")]
        public async Task<ActionResult<SystemTask>> UpdateVersionControlAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScope.VersionControl, Status = Shared.TaskStatus.Pending, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(UpdateVersionControlAsync), task);
        }
    }
}
