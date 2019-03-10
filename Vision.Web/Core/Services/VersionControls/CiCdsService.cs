namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class CiCdsService
    {
        private readonly VisionDbContext context;

        public CiCdsService(VisionDbContext context) => this.context = context;
                
        public async Task<CiCdDto> GetByIdAsync(Guid cicdId)
        {
            CiCd cicd = await context.CiCds.FindAsync(cicdId);
            return new CiCdDto { ApiKey = cicd.ApiKey, CiCdId = cicd.Id, Endpoint = cicd.Endpoint, Kind = cicd.Kind };
        }

        public async Task<CiCdDto> UpdateAsync(CiCdDto dto)
        {
            CiCd cicd = new CiCd { Id = dto.CiCdId, ApiKey = dto.ApiKey, Endpoint = dto.Endpoint, Kind = dto.Kind, Password = dto.Password, Username = dto.Username };
            context.CiCds.Update(cicd);
            await context.SaveChangesAsync();
            return new CiCdDto { ApiKey = cicd.ApiKey, CiCdId = cicd.Id, Endpoint = cicd.Endpoint, Kind = cicd.Kind, Username = cicd.Username, Password = cicd.Password };
        }

        public async Task<CiCdDto> CreateAsync(CiCdDto dto)
        {
            CiCd cicd = new CiCd { Id = Guid.NewGuid(), ApiKey = dto.ApiKey, Endpoint = dto.Endpoint, Kind = dto.Kind, Username = dto.Username, Password = dto.Password };
            context.CiCds.Add(cicd);
            await context.SaveChangesAsync();
            return new CiCdDto { ApiKey = cicd.ApiKey, CiCdId = cicd.Id, Endpoint = cicd.Endpoint, Kind = cicd.Kind, Username = cicd.Username, Password = cicd.Password };
        }

        public async Task<IEnumerable<CiCdDto>> GetAllAsync()
        {
            return await context.CiCds.Select(cicd => new CiCdDto { ApiKey = cicd.ApiKey, CiCdId = cicd.Id, Endpoint = cicd.Endpoint, Kind = cicd.Kind, Username = cicd.Username, Password = cicd.Password }).ToListAsync();
        }
    }
}
