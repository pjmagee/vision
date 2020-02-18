using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vision.Core
{
    public interface IAggregateCICDBuildsProvider
    {
        Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId);
    }
}