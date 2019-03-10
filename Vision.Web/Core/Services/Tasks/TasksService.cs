namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class TasksService
    {
        private readonly VisionDbContext context;

        public TasksService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<SystemTask>> GetAllAsync()
        {
            return await context.Tasks.ToListAsync();
        }

        public async Task<IEnumerable<SystemTask>> GetPendingTasksAsync()
        {
            return await context.Tasks.Where(t => t.Completed == null).ToListAsync();
        }

        public async Task<SystemTask> UpdateAssetAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScopeKind.Asset, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;            
        }

        public async Task<SystemTask> UpdateDependencyAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScopeKind.Dependency, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<SystemTask> UpdateRepositoryAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScopeKind.Repository, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<SystemTask> UpdateVersionControlAsync(Guid id)
        {
            SystemTask task = new SystemTask { Scope = TaskScopeKind.VersionControl, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }
    }
}
