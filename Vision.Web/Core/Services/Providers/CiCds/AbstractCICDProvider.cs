using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public abstract class AbstractCICDProvider : ICICDProvider
    {
        protected readonly VisionDbContext context;
        protected readonly IDataProtector protector;
        protected readonly ILogger<AbstractCICDProvider> logger;
        private readonly IRepositoryMatcher repositoryMatcher;

        public AbstractCICDProvider(VisionDbContext context, IDataProtectionProvider provider, ILogger<AbstractCICDProvider> logger, IRepositoryMatcher repositoryMatcher)
        {
            this.context = context;
            this.protector = provider.CreateProtector("");
            this.logger = logger;
            this.repositoryMatcher = repositoryMatcher;
        }

        protected static string Base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

        public async Task<List<CiCdBuildDto>> GetBuildsByRepositoryIdAsync(Guid repositoryId)
        {
            Repository repository = await context.Repositories.FindAsync(repositoryId);

            List<CiCdBuildDto> builds = new List<CiCdBuildDto>();

            foreach (CiCd cicd in await context.CiCds.Where(cicd => cicd.Kind == Kind).ToListAsync())
            {
                List<CiCdBuildDto> results = await GetBuildsAsync(repository, cicd);
                builds.AddRange(results);
            }

            return builds;
        }

        protected bool IsMatch(string a, string b) => repositoryMatcher.IsMatch(a, b);

        protected abstract Task<List<CiCdBuildDto>> GetBuildsAsync(Repository repository, CiCd cicd);

        protected abstract CiCdKind Kind { get; }

        public bool Supports(CiCdKind kind) => kind == Kind;
    }
}
