using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface ISystemTaskService
    {
        Task<IEnumerable<SystemTask>> GetAllAsync();
        Task<IEnumerable<SystemTask>> GetPendingTasksAsync();
        Task<SystemTask> UpdateAssetAsync(Guid id);
        Task<SystemTask> UpdateDependencyAsync(Guid id);
        Task<SystemTask> UpdateRepositoryAsync(Guid id);
        Task<SystemTask> UpdateVersionControlAsync(Guid id);
    }
}