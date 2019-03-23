namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    
    public class CiCdService : ICiCdService
    {
        private readonly VisionDbContext context;
        private readonly IEncryptionService encryptionService;

        public CiCdService(VisionDbContext context, IEncryptionService encryptionService)
        {
            this.context = context;
            this.encryptionService = encryptionService;
        }

        public async Task<CiCdDto> GetByIdAsync(Guid cicdId)
        {
            CiCd cicd = await context.CiCds.FindAsync(cicdId);

            return new CiCdDto
            {
                CiCdId = cicd.Id,
                Endpoint = cicd.Endpoint,
                Kind = cicd.Kind,
                IsEnabled = cicd.IsEnabled,
                IsGuestEnabled = cicd.IsGuestEnabled,
                ApiKey = cicd.ApiKey,
                Username = cicd.Username,
                Password = cicd.Password
            };
        }

        public async Task<CiCdDto> UpdateAsync(CiCdDto dto)
        {
            encryptionService.Encrypt(dto);

            var cicd = context.CiCds.Find(dto.CiCdId);
            cicd.Endpoint = dto.Endpoint;
            cicd.Kind = dto.Kind;
            cicd.IsEnabled = dto.IsEnabled;
            cicd.IsGuestEnabled = dto.IsGuestEnabled;
            cicd.Password = dto.Password;
            cicd.Username = dto.Username;
            cicd.ApiKey = dto.ApiKey;

            
            context.CiCds.Update(cicd);
            await context.SaveChangesAsync();

            return new CiCdDto
            {
                ApiKey = cicd.ApiKey,
                CiCdId = cicd.Id,
                Endpoint = cicd.Endpoint,
                Kind = cicd.Kind,
                Username = cicd.Username,
                Password = cicd.Password
            };
        }

        public async Task<CiCdDto> CreateAsync(CiCdDto dto)
        {
            encryptionService.Encrypt(dto);

            CiCd cicd = new CiCd
            {
                Id = Guid.NewGuid(),
                Endpoint = dto.Endpoint,
                Kind = dto.Kind,
                IsEnabled = dto.IsEnabled,
                IsGuestEnabled = dto.IsGuestEnabled,
                ApiKey = dto.ApiKey,
                Password = dto.Password,
                Username = dto.Username
            };

            context.CiCds.Add(cicd);
            await context.SaveChangesAsync();

            return new CiCdDto
            {
                ApiKey = cicd.ApiKey,
                CiCdId = cicd.Id,
                Endpoint = cicd.Endpoint,
                Kind = cicd.Kind,
                Username = cicd.Username,
                Password = cicd.Password
            };
        }

        public async Task<IPaginatedList<CiCdDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.CiCds.Select(cicd => new CiCdDto
            {
                ApiKey = cicd.ApiKey,
                CiCdId = cicd.Id,
                Endpoint = cicd.Endpoint,
                Kind = cicd.Kind,
                Username = cicd.Username,
                Password = cicd.Password,
                IsEnabled = cicd.IsEnabled,
                IsGuestEnabled = cicd.IsGuestEnabled
            });            

            return await PaginatedList<CiCdDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
