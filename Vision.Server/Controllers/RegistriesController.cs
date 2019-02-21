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
    public class RegistriesController : ControllerBase
    {
        private readonly VisionDbContext context;

        public RegistriesController(VisionDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<RegistryDto>> GetAllRegistriesAsync()
        {
            var registries = await context.Registries.ToListAsync();

            return await Task.WhenAll(registries.Select(async registry => new RegistryDto
            {
                ApiKey = registry.ApiKey,
                Dependencies = await context.Dependencies.CountAsync(d => d.RegistryId == registry.Id),
                Endpoint = registry.Endpoint,
                Kind = registry.Kind,
                RegistryId = registry.Id
            }));
        }
    }
}
