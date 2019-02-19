using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vision.Core;
using Vision.Shared;

namespace Vision.Server.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class DependenciesController : ControllerBase
    {
        private readonly IDependencyRepository dependencyRepository;
        public DependenciesController(IDependencyRepository dependencyRepository)
        {
            this.dependencyRepository = dependencyRepository;
        }
                
        [HttpGet]
        public async Task<IEnumerable<DependencyDto>> Get()
        {
            var dependencies = await dependencyRepository.GetAllAsync();
            return dependencies.Select(x => new DependencyDto { Assets = x.Assets.Count, Versions = x.Versions.Count, DependencyId = x.Id, Kind = x.Kind, Name = x.Name, RepositoryUrl = x.RepositoryUrl });
        }

        [HttpGet("{dependencyId}")]
        public async Task<DependencyDto> Get(Guid dependencyId)
        {
            var dependency = await dependencyRepository.GetByIdAsync(dependencyId);
            return new DependencyDto { Assets = dependency.Assets.Count, Versions = dependency.Versions.Count, DependencyId = dependency.Id, Kind = dependency.Kind, Name = dependency.Name, RepositoryUrl = dependency.RepositoryUrl };
        }
    }
}
