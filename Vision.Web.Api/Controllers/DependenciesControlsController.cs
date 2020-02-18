using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vision.Core;

namespace Vision.Web.Api.Controllers
{
    [ApiController]
    public class DependenciesControlsController : ControllerBase
    {
        private readonly ILogger<DependenciesControlsController> logger;
        private readonly IVulnerabilityService vulnerabilityService;

        public DependenciesControlsController(ILogger<DependenciesControlsController> logger, IVulnerabilityService vulnerabilityService)
        {
            this.logger = logger;
            this.vulnerabilityService = vulnerabilityService;
        }

        [HttpGet("/dependencies/{dependencyId}/vulnerabilities")]
        public async Task<IEnumerable<VulnerabilityReportDto>> GetVulnerabilities(Guid dependencyId)
        {
            return await vulnerabilityService.GetByDependencyIdAsync(dependencyId);
        }
    }
}
