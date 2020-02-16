using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class RuntimeService : IRuntimeService
    {
        private readonly VisionDbContext context;

        public RuntimeService(VisionDbContext context) => this.context = context;

        public async Task<IPaginatedList<RuntimeDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Runtimes.Select(runtime => new RuntimeDto
            {
                Name = runtime.Name,
                Versions = context.RuntimeVersions.Count(x => x.RuntimeId == runtime.Id),
                RuntimeId = runtime.Id,
                Assets = context.Assets.Count(asset => asset.AssetRuntime.RuntimeVersion.RuntimeId == runtime.Id)
            })
            .OrderByDescending(f => f.Assets);

            return await PaginatedList<RuntimeDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<RuntimeDto> GetRuntimeByIdAsync(Guid runtimeId)
        {
            Runtime runtime = await context.Runtimes.FindAsync(runtimeId);

            return new RuntimeDto
            {
                Name = runtime.Name,
                Versions = context.RuntimeVersions.Count(x => x.RuntimeId == runtime.Id),
                RuntimeId = runtime.Id,
                Assets = context.Assets.Count(asset => asset.AssetRuntime.RuntimeVersion.RuntimeId == runtime.Id)
            };
        }

        public async Task<RuntimeVersionDto> GetVersionByIdAsync(Guid runtimeVersionId)
        {
            RuntimeVersion version = await context.RuntimeVersions.FindAsync(runtimeVersionId);

            return new RuntimeVersionDto
            {
                Version = version.Version,
                RuntimeVersionId = runtimeVersionId,
                RuntimeId = version.Id,
                Assets = context.Assets.Count(asset => asset.AssetRuntime.RuntimeVersionId == version.Id)
            };
        }

        public async Task<IPaginatedList<RuntimeDto>> GetByAssetIdAsync(Guid assetId, int pageIndex = 1, int pageSize = 10)
        {
            var runtimes = context.Runtimes.Where(runtime => context.AssetRuntimes.Any(ar => ar.AssetId == assetId && ar.RuntimeVersion.RuntimeId == runtime.Id));

            var query = runtimes.Select(runtime => new RuntimeDto
            {
                RuntimeId = runtime.Id,
                Name = runtime.Name,
                Versions = context.RuntimeVersions.Count(x => x.RuntimeId == runtime.Id),
                Assets = context.Assets.Count(asset => asset.AssetRuntime.RuntimeVersion.RuntimeId == runtime.Id)
            })
            .OrderByDescending(rt => rt.Assets);

            return await PaginatedList<RuntimeDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RuntimeDto>> GetByRepositoryIdAsync(Guid repositoryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Runtimes
                .Where(rt => context.AssetRuntimes.Any(assetRuntime => assetRuntime.RuntimeVersion.RuntimeId == rt.Id && context.Assets.Any(a => a.RepositoryId == repositoryId && assetRuntime.AssetId == a.Id)))
                .Select(rt => new RuntimeDto
                {
                    Assets = context.Assets.Where(asset => asset.RepositoryId == repositoryId).Count(asset => asset.AssetRuntime.RuntimeVersion.RuntimeId == rt.Id),
                    RuntimeId = rt.Id,
                    Name = rt.Name,
                    Versions = context.RuntimeVersions.Count(rtv => rtv.RuntimeId == rt.Id)
                })
                .OrderByDescending(rt => rt.Assets);

            return await PaginatedList<RuntimeDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<IPaginatedList<RuntimeVersionDto>> GetByRuntimeIdAsync(Guid runtimeId, int pageIndex = 1, int pageSize = 10)
        {
            var query = context.RuntimeVersions
                .Where(rtv => rtv.RuntimeId == runtimeId)
                .Select(rtv => new RuntimeVersionDto
                {
                    Assets = context.Assets.Count(asset => asset.AssetRuntime.RuntimeVersionId == rtv.Id),
                    RuntimeId = runtimeId,
                    RuntimeVersionId = rtv.Id,
                    Version = rtv.Version
                })
                .OrderByDescending(rtv => rtv.Assets);

            return await PaginatedList<RuntimeVersionDto>.CreateAsync(query, pageIndex, pageSize);
        }
    }
}
