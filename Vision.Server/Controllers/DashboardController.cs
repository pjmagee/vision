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

        [HttpGet("/metrics/assets/{count?}")]
        public async Task<IEnumerable<MetricDto<AssetDto>>> GetAssetsMetrics(int count = 5)
        {
            IQueryable<Asset> orderedByLargest = context.Assets.OrderByDescending(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id));
            // orderedByLargest.TakeLast(5).Select(asset => new AssetDto { AssetId = asset.Id, Asset = asset.Path, Repository = asset.Repository.Url, RepositoryId = asset.RepositoryId })

            return new MetricDto<AssetDto>[]
            {
                new MetricDto<AssetDto>(
                    MetricsKind.Info,
                    "Top 5 smallest assets",
                    await orderedByLargest.TakeLast(5).Select(asset => new AssetDto { AssetId = asset.Id, Asset = asset.Path, Repository = asset.Repository.Url, RepositoryId = asset.RepositoryId }).ToListAsync()),

                new MetricDto<AssetDto>(
                    MetricsKind.Info,
                    "Top 5 largest assets",
                    await orderedByLargest.Take(5).Select(asset => new AssetDto { AssetId = asset.Id, Asset = asset.Path, Repository = asset.Repository.Url, RepositoryId = asset.RepositoryId }).ToListAsync())
            };
        }

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
