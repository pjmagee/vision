namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class VersionControlsService
    {
        private readonly VisionDbContext context;

        public VersionControlsService(VisionDbContext context) => this.context = context;

        public async Task<IEnumerable<VersionControlDto>> GetAllAsync()
        {
            return await context.VersionControls.Select(versionControl => new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VersionControlId = versionControl.Id,
                Repositories = context.Repositories.Count(repository => repository.VersionControlId == versionControl.Id)
            }).ToListAsync();
        }


        public async Task<ActionResult<VersionControlDto>> CreateVersionControl([FromBody] VersionControlDto post)
        {
            VersionControl versionControl = new VersionControl { Id = Guid.NewGuid(), ApiKey = post.ApiKey, Endpoint = post.Endpoint, Kind = post.Kind };

            context.VersionControls.Add(versionControl);
            await context.SaveChangesAsync();

            return new VersionControlDto { Endpoint = versionControl.Endpoint, ApiKey = versionControl.ApiKey, Kind = versionControl.Kind, VersionControlId = versionControl.Id };
        }

        public async Task<VersionControlDto> GetVersionControlByIdAsync(Guid versionControlId)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

            return new VersionControlDto
            {
                Endpoint = versionControl.Endpoint,
                ApiKey = versionControl.ApiKey,
                Kind = versionControl.Kind,
                VersionControlId = versionControl.Id,
                Repositories = await context.Repositories.CountAsync(x => x.VersionControlId == versionControl.Id)
            };
        }

        public async Task<IEnumerable<MetricItems<RepositoryDto>>> GetRepositoriesMetricsByVersionControlIdAsync(Guid versionControlId)
        {
            IQueryable<Repository> orderedByLargest = context.Repositories.Where(repository => repository.VersionControlId == versionControlId).OrderBy(repository => context.Assets.Count(asset => asset.RepositoryId == repository.Id));

            return new MetricItems<RepositoryDto>[]
            {
                new MetricItems<RepositoryDto>(MetricAlertKind.Standard, MetricCategoryKind.Repositories, "Top 5 smallest repositories", await orderedByLargest.TakeLast(5).Select(repository => new RepositoryDto {  }).ToArrayAsync()),
                new MetricItems<RepositoryDto>(MetricAlertKind.Standard, MetricCategoryKind.Repositories, "Top 5 largest repositories", await orderedByLargest.Take(5).Select(repository => new RepositoryDto {  }).ToArrayAsync())
            };
        }

        public async Task<IEnumerable<RepositoryDto>> GetRepositoriesByIdAsync(Guid versionControlId)
        {
            var repositories = await context.Repositories.Where(repository => repository.VersionControlId == versionControlId).ToListAsync();
            return repositories.Select(x => new RepositoryDto { VersionControlId = x.VersionControlId, Assets = x.Assets.Count, Url = x.Url, WebUrl = x.WebUrl, RepositoryId = x.Id });
        }

        public async Task<IEnumerable<AssetDto>> GetAssetsByVersionControlIdAsync(Guid versionControlId)
        {
            VersionControl versionControl = await context.VersionControls.FindAsync(versionControlId);

            return await context.Assets
                    .Where(asset => context.Repositories.Any(repository => repository.VersionControlId == versionControlId && asset.RepositoryId == repository.Id))
                    .Select(asset => new AssetDto
                    {
                        Asset = asset.Path,
                        AssetId = asset.Id,
                        Dependencies = context.AssetDependencies.Count(ad => ad.AssetId == asset.Id),
                        Repository = asset.Repository.Url,
                        RepositoryId = asset.RepositoryId,
                        VersionControlId = versionControlId
                    })
                    .ToListAsync();
        }


    }
}
