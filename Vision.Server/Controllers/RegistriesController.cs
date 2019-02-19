using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class RegistriesController : ControllerBase
    {
        private readonly IRegistryRepository registryRepository;

        public RegistriesController(IRegistryRepository registryRepository)
        {
            this.registryRepository = registryRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<RegistryDto>> Get()
        {
            var registries = await registryRepository.GetAllAsync();

            return registries.Select(x => new RegistryDto
            {
                ApiKey = x.ApiKey,
                Dependencies = x.Dependencies.Count,
                Endpoint = x.Endpoint,
                IsDocker = x.IsDocker,
                IsNuGet = x.IsNuGet,
                IsMaven = x.IsMaven,
                IsNpm = x.IsNpm,
                IsPublic = x.IsPublic,
                IsPyPi = x.IsPyPi,
                IsRubygem = x.IsRubyGem,
                RegistryId = x.Id
            });
        }
    }
}
