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



        [HttpGet("/metrics/counts")]
        public async Task<IEnumerable<MetricItem>> GetCounts()
        {
            IEnumerable<Task<MetricItem>> dependencies = AppHelper.Kinds.Select(async kind => new MetricItem(MetricsKind.Info, $"{kind} Count", await context.Dependencies.CountAsync(x => x.Kind == kind)));
            IEnumerable<Task<MetricItem>> assets = AppHelper.Kinds.Select(async kind => new MetricItem(MetricsKind.Info, $"{kind} Count", await context.Assets.CountAsync(x => x.Kind == kind)));

            var otherCounts = new MetricItem[]
            {
                   new MetricItem(MetricsKind.Info, $"Version controls Count", await context.VersionControls.CountAsync()),
                   new MetricItem(MetricsKind.Info, $"Repositories Count", await context.Repositories.CountAsync()),
                   new MetricItem(MetricsKind.Info, $"Dependencies Count", await context.Dependencies.CountAsync()),
                   new MetricItem(MetricsKind.Info, $"Registries Count", await context.Registries.CountAsync())
            };

            return (await Task.WhenAll(dependencies.Concat(assets).ToArray())).Concat(otherCounts);

            //IEnumerable<Task<MetricItem>> leastTasks = AppHelper.Kinds.Select(async kind => new MetricItem(MetricsKind.Info, $"{kind} most used", await context.Dependencies.Where(d => d.Kind == kind).OrderBy(dependency => context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id)).Take(10).Select(d => d.Name).ToArrayAsync()));
            //IEnumerable<Task<MetricItem>> mostTasks = AppHelper.Kinds.Select(async kind => new MetricItem(MetricsKind.Info, $"{kind} most used", await context.Dependencies.Where(d => d.Kind == kind).OrderByDescending(dependency => context.AssetDependencies.Count(ad => ad.DependencyId == dependency.Id)).Take(10).Select(d => d.Name).ToArrayAsync()));

            //return await Task.WhenAll(countTasks.Concat(orderedTasks).ToArray());



            // dependencies ordered by most used
            // dependencies ordered by least used
        }

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
