using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core.Services.Build
{
    public interface IBuildService
    {
        Task<IEnumerable<Build>> GetBuildsByRepositoryAsync(Guid repositoryId);
    }
}
