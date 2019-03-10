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
                
        public async Task<CiCd> CreateCiCd(CiCdDto post)
        {
            CiCd cicd = new CiCd { Id = Guid.NewGuid(), ApiKey = post.ApiKey, Endpoint = post.Endpoint, Kind = post.Kind };
            context.CiCds.Add(cicd);
            await context.SaveChangesAsync();
            return cicd;
        }

        public async Task<IEnumerable<CiCdDto>> GetAllAsync()
        {
            return await context.CiCds.Select(cicd => new CiCdDto { ApiKey = cicd.ApiKey, CiCdId = cicd.Id, Endpoint = cicd.Endpoint, Kind = cicd.Kind }).ToListAsync();
        }
    }
}
