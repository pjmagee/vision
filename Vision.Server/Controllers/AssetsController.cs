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
    public class AssetsController : ControllerBase
    {
        private readonly VisionDbContext context;

        public AssetsController(VisionDbContext context)
        {
            this.context = context;
        }

        [Route("metrics"), HttpGet("/metrics")]
        public async Task<IEnumerable<AssetMetricDto>> GetAssetMetricsAsync()
        {
            List<AssetMetricDto> metrics = new List<AssetMetricDto>();

            foreach (var kind in Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>())
            {
                IQueryable<Asset> assets = context.Assets.Where(asset => context.Dependencies.Where(dependency => dependency.Kind == kind).Any(dependency => context.AssetDependencies.Any(ad => ad.AssetId == asset.Id && ad.DependencyId == dependency.Id)));

                metrics.Add(new AssetMetricDto { Kind = MetricsKind.Info, Title = $"{kind} assets with most dependencies", Items = await assets.OrderByDescending(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id)).Take(10).Select(asset => new AssetDto { Repository = asset.Repository.Url, Asset = asset.Path, Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id) }).ToArrayAsync() });
                metrics.Add(new AssetMetricDto { Kind = MetricsKind.Info, Title = $"{kind} assets with least dependencies", Items = await assets.OrderBy(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id)).Take(10).Select(asset => new AssetDto { Repository = asset.Repository.Url, Asset = asset.Path, Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id) }).ToArrayAsync() });
            }

            return metrics;

            //var assetsOrderedByDependencies = context.Assets
            //    .OrderByDescending(asset => context.AssetDependencies.Count(ad => ad.AssetId == asset.Id))
            //    .Select(asset => new AssetDto
            //    {
            //        AssetId = asset.Id,
            //        Asset = asset.Path,
            //        Repository = asset.Repository.Url,
            //        RepositoryId = asset.RepositoryId,
            //        Dependencies = context.AssetDependencies.Count(assetDependency => assetDependency.AssetId == asset.Id)
            //    });

            //var count = assetsOrderedByDependencies.Count();

            //var assets = from version in context.DependencyVersions.AsNoTracking()                         
            //             from assetDependency in context.AssetDependencies.AsNoTracking()
            //             from asset in context.Assets.AsNoTracking()
            //             where assetDependency.DependencyVersionId == version.Id && assetDependency.AssetId == asset.Id
            //             //let latest = (from assetDependency in context.AssetDependencies.AsNoTracking()
            //             //              from version in context.DependencyVersions.AsNoTracking()
            //             //              where version.IsLatest && assetDependency.DependencyVersionId == version.Id)
                         
            //             select asset;            
 
            //return new AssetMetricDto[]
            //{
            //    new AssetMetricDto(MetricsKind.Info, "Most dependencies", await assetsOrderedByDependencies.Take(5).ToArrayAsync()),
            //    new AssetMetricDto(MetricsKind.Info, "Least dependencies", await assetsOrderedByDependencies.Skip(count - 5).Take(5).ToArrayAsync()),
            //    //new MetricDto<AssetDto>(MetricsKind.Info, "Most updated dependencies", await assetsByDependencyCount.TakeLast(5).ToListAsync()),
            //    //new MetricDto<AssetDto>(MetricsKind.Info, "Least updated dependencies", await assetsByDependencyCount.TakeLast(5).ToListAsync()),
            //};
        }

        [HttpGet("{assetId:guid}")]
        public async Task<AssetDto> GetAssetByIdAsync(Guid assetId)
        {
            Asset asset = await context.Assets.FindAsync(assetId);

            return new AssetDto
            {
                AssetId = asset.Id,
                Dependencies = await context.AssetDependencies.CountAsync(x => x.AssetId == assetId),
                Asset = asset.Path,
                RepositoryId = asset.RepositoryId
            };
        }

        [HttpGet("{assetId}/dependencies")]
        public async Task<IEnumerable<AssetDependencyDto>> GetDependenciesByAssetIdAsync(Guid assetId)
        {
            return await context.AssetDependencies.Where(assetDependency => assetDependency.AssetId == assetId).Select(assetDependency => new AssetDependencyDto
            {
                Repository = assetDependency.Asset.Repository.WebUrl,
                Dependency = assetDependency.Dependency.Name,
                Version = assetDependency.DependencyVersion.Version,
                AssetId = assetDependency.AssetId,
                Asset = assetDependency.Asset.Path,
                DependencyId = assetDependency.DependencyId,
                DependencyVersionId = assetDependency.DependencyVersionId,
                IsLatest = assetDependency.DependencyVersion.IsLatest,
                RepositoryId = assetDependency.Asset.RepositoryId
            })
            .ToListAsync();
        }

        [HttpGet("{assetId}/frameworks")]
        public async Task<IEnumerable<AssetFrameworkDto>> GetFrameworksByAssetId(Guid assetId)
        {
            return await context.AssetFrameworks
                .Where(x => x.AssetId == assetId)
                .Select(x => new AssetFrameworkDto { FrameworkId = x.FrameworkId, Name = x.Framework.Version, AssetId = x.AssetId })
                .ToListAsync();
        }

        [HttpPost("/search")]
        public async Task<IEnumerable<AssetDto>> GetAssetByIdAsync([FromBody] AssetSearch assetSearch)
        {
            var dependencyIds = assetSearch.DependencyIds;
            var versionControlIds = assetSearch.VersionControlIds;

            IQueryable<Asset> query = context.Assets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(assetSearch.Name))
            {
                query = query.Where(asset => asset.Path.Contains(assetSearch.Name));
            }

            if (dependencyIds.Any())
            {
                query = query.Where(asset => versionControlIds.Contains(asset.Repository.VersionControlId));
            }

            if (versionControlIds.Any())
            {
                query = query.Where(asset => context.AssetDependencies.Any(assetDependency => assetDependency.AssetId == asset.Id && dependencyIds.Contains(assetDependency.Id)));
            }

            return await query.Select(asset => new AssetDto { AssetId = asset.Id, Dependencies = context.AssetDependencies.Count(x => x.AssetId == x.Id), Asset = asset.Path, RepositoryId = asset.RepositoryId }).ToListAsync();
        }
    }
}
