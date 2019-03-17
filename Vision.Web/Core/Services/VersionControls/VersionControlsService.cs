namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.EntityFrameworkCore;

    public class VersionControlsService
    {
        private readonly VisionDbContext context;
        private readonly IDataProtector protector;

        public VersionControlsService(VisionDbContext context, IDataProtectionProvider provider)
        {
            this.context = context;
            this.protector = provider.CreateProtector("Auth");
        }

        public async Task<PaginatedList<VersionControlDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query =context.VersionControls.Select(versionControl => new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VersionControlId = versionControl.Id,
                Repositories = context.Repositories.Count(repository => repository.VersionControlId == versionControl.Id)
            })
            .OrderByDescending(vcs => vcs.Repositories);

            return await PaginatedList<VersionControlDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<VersionControlDto> CreateVersionControl(VersionControlDto post)
        {
            VersionControl versionControl = new VersionControl
            {
                Id = Guid.NewGuid(),
                ApiKey = !string.IsNullOrWhiteSpace(post.ApiKey) ? protector.Protect(post.ApiKey) : null,
                Endpoint = post.Endpoint,
                Kind = post.Kind
            };

            context.VersionControls.Add(versionControl);
            await context.SaveChangesAsync();

            return new VersionControlDto { Endpoint = versionControl.Endpoint, ApiKey = versionControl.ApiKey, Kind = versionControl.Kind, VersionControlId = versionControl.Id };
        }

        public async Task<VersionControlDto> GetByIdAsync(Guid versionControlId)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

            return new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VersionControlId = versionControl.Id,
                Repositories = await context.Repositories.CountAsync(x => x.VersionControlId == versionControl.Id)
            };
        }
    }
}
