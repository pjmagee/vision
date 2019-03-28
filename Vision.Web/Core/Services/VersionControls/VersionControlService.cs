namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class VersionControlService : IVersionControlService
    {
        private readonly VisionDbContext context;
        private readonly IEncryptionService encryptionService;

        public VersionControlService(VisionDbContext context, IEncryptionService encryptionService)
        {
            this.context = context;
            this.encryptionService = encryptionService;
        }

        public async Task<IPaginatedList<VersionControlDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.VersionControls.Select(versionControl => new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind =  versionControl.Kind,
                VersionControlId = versionControl.Id,
                IsEnabled = versionControl.IsEnabled,
                Repositories = context.Repositories.Count(repository => repository.VersionControlId == versionControl.Id)
            })
            .OrderByDescending(vcs => vcs.Repositories);

            return await PaginatedList<VersionControlDto>.CreateAsync(query.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<VersionControlDto> CreateVersionControl(VersionControlDto dto)
        {
            encryptionService.Encrypt(dto);

            VersionControl versionControl = new VersionControl
            {
                Id = Guid.NewGuid(),
                ApiKey = dto.ApiKey,
                Endpoint = dto.Endpoint,
                Kind = dto.Kind,
                IsEnabled = dto.IsEnabled
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

        public async Task<VersionControlDto> UpdateAsync(VersionControlDto dto)
        {
            encryptionService.Encrypt(dto);

            VersionControl versionControl = context.VersionControls.Find(dto.VersionControlId);
            versionControl.Endpoint = dto.Endpoint;
            versionControl.Kind = dto.Kind;
            versionControl.IsEnabled = dto.IsEnabled;
            versionControl.ApiKey = dto.ApiKey;

            context.VersionControls.Update(versionControl);
            await context.SaveChangesAsync();

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
