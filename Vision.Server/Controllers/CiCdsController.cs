using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ResponseCache(Duration = 30)]
    [ApiController, Route("api/[controller]"), Produces("application/json"), ApiConventionType(typeof(DefaultApiConventions))]
    public class CiCdsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public CiCdsController(VisionDbContext context) => this.context = context;

        [HttpPost, Route("create")]
        public async Task<IActionResult> PostCiCdAsync([FromBody] CiCdDto post)
        {
            CiCd cicd = new CiCd { Id = Guid.NewGuid(), ApiKey = post.ApiKey, Endpoint = post.Endpoint, Kind = post.Kind };

            context.CiCds.Add(cicd);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostCiCdAsync), new CiCdDto { Endpoint = post.Endpoint, ApiKey = post.ApiKey, Kind = post.Kind, CiCdId = post.CiCdId });
        }

        [HttpGet]
        public async Task<IEnumerable<CiCdDto>> GetAllAsync()
        {
            return await context.CiCds.Select(cicd => new CiCdDto { ApiKey = cicd.ApiKey, CiCdId = cicd.Id, Endpoint = cicd.Endpoint, Kind = cicd.Kind }).ToListAsync();
        }
    }
}
