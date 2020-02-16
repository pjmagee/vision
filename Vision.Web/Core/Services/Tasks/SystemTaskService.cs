using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class SystemTaskService : ISystemTaskService
    {
        private readonly VisionDbContext context;

        public SystemTaskService(VisionDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<RefreshTask>> GetAllAsync()
        {
            return await context.Tasks.ToListAsync();
        }

        public async Task<IEnumerable<RefreshTask>> GetPendingTasksAsync()
        {
            return await context.Tasks.Where(t => t.Completed == null).ToListAsync();
        }

        public async Task<RefreshTask> UpdateAssetAsync(Guid id)
        {
            RefreshTask task = new RefreshTask { Scope = TaskScopeKind.Asset, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<RefreshTask> UpdateDependencyAsync(Guid id)
        {
            RefreshTask task = new RefreshTask { Scope = TaskScopeKind.Dependency, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<RefreshTask> UpdateRepositoryAsync(Guid id)
        {
            RefreshTask task = new RefreshTask { Scope = TaskScopeKind.Repository, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }

        public async Task<RefreshTask> UpdateVersionControlAsync(Guid id)
        {
            RefreshTask task = new RefreshTask { Scope = TaskScopeKind.VersionControl, TargetId = id };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            return task;
        }
    }
}
