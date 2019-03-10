namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICICDBuildsService
    {
        CiCdKind Kind { get; }
        Task<IEnumerable<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId);
    }
}
