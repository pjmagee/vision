using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    public abstract class AbstractCICDProvider : ICiCdProvider
    {
        protected readonly ILogger<AbstractCICDProvider> logger;
        protected readonly IRepositoryMatcher matcher;

        public AbstractCICDProvider(ILogger<AbstractCICDProvider> logger, IRepositoryMatcher matcher)
        {
            this.logger = logger;
            this.matcher = matcher;
        }

        protected static string Base64Encode(string text) => Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

        public async Task<List<CiCdBuildDto>> GetBuildsAsync(RepositoryDto repository, CiCdDto cicd)
        {
            using (var scope = logger.BeginScope($"[{repository.Url}]::[{cicd.Endpoint}]"))
            {
                try
                {
                    return await TryGetBuildsAsync(repository, cicd);
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"[{repository.Url}]::[{cicd.Endpoint}]::ERROR");
                    return null;
                }
            }
        }

        protected abstract Task<List<CiCdBuildDto>> TryGetBuildsAsync(RepositoryDto repository, CiCdDto cicd);

        protected abstract CiCdKind Kind { get; }

        public bool Supports(CiCdKind kind) => kind == Kind;
    }
}
