namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.DataProtection;

    public class CiCdsService
    {
        private readonly VisionDbContext context;        
        private readonly IDataProtector protector;

        public CiCdsService(VisionDbContext context, IDataProtectionProvider provider)
        {
            this.context = context;
            this.protector = provider.CreateProtector("Auth");
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
            var cicd = context.CiCds.Find(dto.CiCdId);
            cicd.Endpoint = dto.Endpoint;
            cicd.Kind = dto.Kind;
            cicd.IsEnabled = dto.IsEnabled;
            cicd.IsGuestEnabled = dto.IsGuestEnabled;
            cicd.ApiKey = string.IsNullOrWhiteSpace(dto.ApiKey) ? null : protector.Protect(dto.ApiKey);
            cicd.Username = string.IsNullOrWhiteSpace(dto.Username) ? null : protector.Protect(dto.Username);
            cicd.Password = string.IsNullOrWhiteSpace(dto.Password) ? null : protector.Protect(dto.Password);

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
            CiCd cicd = new CiCd
            {
                Id = Guid.NewGuid(),               
                Endpoint = dto.Endpoint,
                Kind = dto.Kind,
                IsEnabled = dto.IsEnabled,
                IsGuestEnabled = dto.IsGuestEnabled,
                ApiKey = string.IsNullOrWhiteSpace(dto.ApiKey) ? null : protector.Protect(dto.ApiKey),
                Username = string.IsNullOrWhiteSpace(dto.Username) ? null : protector.Protect(dto.Username),
                Password = string.IsNullOrWhiteSpace(dto.Password) ? null : protector.Protect(dto.Password)
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

        public async Task<PaginatedList<CiCdDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.CiCds.Select(cicd => new CiCdDto
            {
                ApiKey = cicd.ApiKey,
                CiCdId = cicd.Id,
                Endpoint = cicd.Endpoint,
                Kind = cicd.Kind,
                Username = cicd.Username,
                Password = cicd.Password
            });

            return await PaginatedList<CiCdDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
