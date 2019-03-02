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

        [HttpGet, Route("metrics/counts")]
        public async Task<IEnumerable<MetricItem>> GetCountsAsync()
        {
            IEnumerable<Task<MetricItem>> dependencies =
                AppHelper.DependencyKinds.Select(async kind => new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, kind, $"{kind} dependencies", await context.Dependencies.CountAsync(x => x.Kind == kind)));

            IEnumerable<Task<MetricItem>> assets =
                AppHelper.DependencyKinds.Select(async kind => new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Assets, kind, $"{kind} assets", await context.Assets.CountAsync(x => x.Kind == kind)));

            var otherCounts = new MetricItem[]
            {
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.VersionControls, $"Version control systems", await context.VersionControls.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Registries, $"Registry sources", await context.Registries.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.CiCds, $"CI/CD sources", await context.CiCds.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Repositories, $"Repositories", await context.Repositories.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Assets, $"Assets", await context.Assets.CountAsync()),
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Dependencies, $"Dependencies", await context.Dependencies.CountAsync()),                
                new MetricItem(MetricAlertKind.Standard, MetricCategoryKind.Frameworks, $"Frameworks", await context.Frameworks.CountAsync()),
            };

            return otherCounts.Concat(await Task.WhenAll(dependencies.Concat(assets).ToArray()));
        }     
    }
}
