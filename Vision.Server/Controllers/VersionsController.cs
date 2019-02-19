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
    public class VersionsController : ControllerBase
    {
        private readonly IDependencyVersionRepository versionRegistry;
        public VersionsController(IDependencyVersionRepository versionRegistry)
        {
            this.versionRegistry = versionRegistry;
        }

        [HttpGet("{dependencyId}")]
        public async Task<IEnumerable<DependencyVersionDto>> Get(Guid dependencyId)
        {
            var versions = await versionRegistry.GetVersionsByDependencyIdAsync(dependencyId);

            return versions.Select(x => new DependencyVersionDto { DependencyId = x.DependencyId, DependencyVersionId = x.Id, IsVulnerable = x.IsVulnerable, Version = x.Version, VulnerabilityUrl = x.VulnerabilityUrl });
        }
    }
}
