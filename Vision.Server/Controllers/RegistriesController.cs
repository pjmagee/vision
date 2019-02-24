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
    public class RegistriesController : ControllerBase
    {
        private readonly VisionDbContext context;

        public RegistriesController(VisionDbContext context) => this.context = context;

        [HttpGet]
        public async Task<IEnumerable<RegistryDto>> GetAllRegistriesAsync()
        {
            return await context.Registries.Select(registry => new RegistryDto
            {
                ApiKey = registry.ApiKey,
                Dependencies = context.Dependencies.Count(d => d.RegistryId == registry.Id),
                Endpoint = registry.Endpoint,
                Kind = registry.Kind,
                RegistryId = registry.Id
            })
            .ToListAsync();
        }

        [HttpGet("{registryId}/dependencies")]
        public async Task<IEnumerable<DependencyDto>> GetDependenciesByRegistry(Guid registryId)
        {
            return await context.Dependencies
                    .Where(d => d.RegistryId == registryId)
                    .Select(d => new DependencyDto
                    {
                        Name = d.Name,
                        RepositoryUrl = d.RepositoryUrl,
                        Versions = context.DependencyVersions.Count(x => x.DependencyId == d.Id),
                        Assets = context.AssetDependencies.Count(x => x.DependencyId == d.Id),
                        DependencyId = d.Id,
                        Kind = d.Kind                    
                    }).ToListAsync();
        }
    }
}
