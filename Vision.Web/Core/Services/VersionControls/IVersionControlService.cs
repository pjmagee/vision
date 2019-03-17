using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IVersionControlService
    {
        Task<VersionControlDto> CreateVersionControl(VersionControlDto post);
        Task<IPaginatedList<VersionControlDto>> GetAsync(int pageIndex = 1, int pageSize = 10);
        Task<VersionControlDto> GetByIdAsync(Guid versionControlId);
    }
}