using System;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public interface IVersionControlService
    {
        Task<VersionControlDto> CreateVersionControl(VersionControlDto versionControl);
        Task<IPaginatedList<VersionControlDto>> GetAsync(int pageIndex = 1, int pageSize = 10);
        Task<VersionControlDto> GetByIdAsync(Guid VcsId);
        Task<VersionControlDto> UpdateAsync(VersionControlDto versionControl);
    }
}