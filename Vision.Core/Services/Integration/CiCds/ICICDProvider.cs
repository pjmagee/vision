namespace Vision.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICiCdProvider
    {
        bool Supports(CiCdKind Kind);
        Task<List<CiCdBuildDto>> GetBuildsAsync(RepositoryDto repository, CiCdDto cicd);
    }
}
