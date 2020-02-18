using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vision.Core;

namespace Vision.Core
{
    public interface ISystemTaskService
    {
        Task<IEnumerable<RefreshTask>> GetAllAsync();
        Task<IEnumerable<RefreshTask>> GetPendingTasksAsync();
        Task<RefreshTask> UpdateAssetAsync(Guid id);
        Task<RefreshTask> UpdateDependencyAsync(Guid id);
        Task<RefreshTask> UpdateRepositoryAsync(Guid id);
        Task<RefreshTask> UpdateVersionControlAsync(Guid id);
    }
}