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
    public class DashboardController : ControllerBase
    {
        private readonly VisionDbContext context;

        public DashboardController(VisionDbContext context) => this.context = context;

        

        //[HttpGet("/metrics/dependencies")]
        //public async Task <IEnumerable<MetricDto>> GetDependenciesMetrics()
        //{

        //}

        //[HttpGet("/metrics/frameworks")]
        //public async Task<IEnumerable<MetricDto>> GetFrameworksMetrics()
        //{

        //}

        //[HttpGet("/metrics/registries")]
        //public async Task<IEnumerable<MetricDto>> GetRegistriesMetrics()
        //{

        //}
    }
}
