using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vision.Core;

namespace Vision.Web.Api.Controllers
{
    [ApiController]
    public class AssetsControlsController : ControllerBase
    {
        private readonly ILogger<AssetsControlsController> logger;
        private readonly IAssetService assetService;
        private readonly IAssetDependencyService assetDependencyService;

        public AssetsControlsController(
            ILogger<AssetsControlsController> logger,
            IAssetService assetService,
            IAssetDependencyService assetDependencyService)
        {
            this.logger = logger;
            this.assetService = assetService;
            this.assetDependencyService = assetDependencyService;
        }

        [HttpGet("assets/{assetId}")]
        public async Task<AssetDto> GetAssetById(Guid assetId)
        {
            return await assetService.GetByIdAsync(assetId);
        }

        [HttpGet("assets/{assetId}/dependencies")]
        public async Task<IEnumerable<AssetDependencyDto>> GetDependenciesByAssetId(Guid assetId, int pageIndex = 1, int pageSize = 10)
        {
            return await assetDependencyService.GetByAssetIdAsync(assetId, pageIndex, pageSize);
        }
    }
}
