using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class RegistriesController : ControllerBase
    {
        private readonly VisionDbContext context;

        public RegistriesController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<RegistryDto>> Get()
        {
            var registries = await context.Registries.ToListAsync();

            return registries.Select(x => new RegistryDto
            {
                ApiKey = x.ApiKey,
                Dependencies = x.Dependencies.Count,
                Endpoint = x.Endpoint,
                Kind = x.Kind,
                RegistryId = x.Id
            });
        }
    }
}
