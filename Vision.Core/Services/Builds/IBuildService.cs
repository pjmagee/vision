using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core.Services.Builds
{
    public interface IBuildService
    {
        Task<IEnumerable<Build>> GetBuildsByRepositoryIdAsync(Guid repositoryId);
    }
}
