using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Core
{
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
            var query = context.VcsSources.Select(versionControl => new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind =  versionControl.Kind,
                VcsId = versionControl.Id,
                IsEnabled = versionControl.IsEnabled,
                Repositories = context.VcsRepositories.Count(repository => repository.VcsId == versionControl.Id)
            })
            .OrderByDescending(vcs => vcs.Repositories);

            return await PaginatedList<VersionControlDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<VersionControlDto> CreateVersionControl(VersionControlDto dto)
        {
            encryptionService.Encrypt(dto);

            Vcs vcs = new Vcs
            {
                Id = Guid.NewGuid(),
                ApiKey = dto.ApiKey,
                Endpoint = dto.Endpoint,
                Kind = dto.Kind,
                IsEnabled = dto.IsEnabled
            };

            context.VcsSources.Add(vcs);
            await context.SaveChangesAsync();

            return new VersionControlDto { Endpoint = vcs.Endpoint, ApiKey = vcs.ApiKey, Kind = vcs.Kind, VcsId = vcs.Id };
        }

        public async Task<VersionControlDto> GetByIdAsync(Guid VcsId)
        {
            Vcs vcs = await context.VcsSources.FindAsync(VcsId);

            return new VersionControlDto
            {
                Endpoint = vcs.Endpoint,
                ApiKey = vcs.ApiKey,
                Kind = vcs.Kind,
                VcsId = vcs.Id,
                Repositories = context.VcsRepositories.Count(x => x.VcsId == vcs.Id)
            };
        }

        public async Task<VersionControlDto> UpdateAsync(VersionControlDto dto)
        {
            encryptionService.Encrypt(dto);

            Vcs versionControl = context.VcsSources.Find(dto.VcsId);
            versionControl.Endpoint = dto.Endpoint;
            versionControl.Kind = dto.Kind;
            versionControl.IsEnabled = dto.IsEnabled;
            versionControl.ApiKey = dto.ApiKey;

            context.VcsSources.Update(versionControl);
            await context.SaveChangesAsync();

            return new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VcsId = versionControl.Id,
                Repositories = context.VcsRepositories.Count(x => x.VcsId == versionControl.Id)
            };
        }
    }
}
