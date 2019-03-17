namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICICDProvider
    {
        bool Supports(CiCdKind Kind);
        Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId);
    }
}
