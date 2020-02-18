using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vision.Core;

namespace Vision.Web.Api.Controllers
{

    [ApiController]
    public class VersionControlsController : ControllerBase
    {
        private readonly ILogger<VersionControlsController> logger;
        private readonly IVersionControlService versionControlService;
        private readonly IRepositoryService repositoryService;
        private const string search = null;

        public VersionControlsController(
            ILogger<VersionControlsController> logger,
            IVersionControlService versionControlService,
            IRepositoryService repositoryService,
            IAssetService assetService,
            IDependencyService dependencyService,
            IAssetDependencyService assetDependencyService,
            IVulnerabilityService vulnerabilityService)
        {
            this.logger = logger;
            this.versionControlService = versionControlService;
            this.repositoryService = repositoryService;
        }

        [HttpGet("/vcs")]
        public async Task<IEnumerable<VersionControlDto>> GetVersionControls(int pageIndex = 1, int pageSize = 10)
        {
            return await versionControlService.GetAsync(pageIndex, pageSize);
        }

        [HttpGet("/vcs/{vcsId}")]
        public async Task<VersionControlDto> GetVcsById(Guid vcsId)
        {
            return await versionControlService.GetByIdAsync(vcsId);
        }

        [HttpGet("/vcs/{vcsId}/repositories")]
        public async Task<IEnumerable<RepositoryDto>> GetRepositoriesByVersionControlId(Guid vcsId, int pageIndex = 1, int pageSize = 10)
        {
            return await repositoryService.GetByVcsIdAsync(vcsId, search, pageIndex, pageSize);
        }
    }
}
