using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.Core.Services.Builds
{
    public interface ICiCdChecker
    {
        Task<IEnumerable<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId);
    }
}
