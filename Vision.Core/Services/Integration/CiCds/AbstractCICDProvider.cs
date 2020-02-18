using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Vision.Core
{
    public abstract class AbstractCICDProvider : ICiCdProvider
    {
        protected readonly ILogger<AbstractCICDProvider> logger;
        protected readonly IRepositoryMatcher matcher;
        private readonly IMemoryCache cache;
        
        protected HttpClient GetHttpClient(CiCdDto cicd)
        {
            if (!cache.TryGetValue(cicd.Endpoint, out HttpClient client))
            {
                client = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (req, cert, chain, policy) => true });
                AddAuthentication(client, cicd);
                cache.Set(cicd.Endpoint, client);
            }

            return client;
        }

        protected abstract void AddAuthentication(HttpClient client, CiCdDto cicd);

        public AbstractCICDProvider(ILogger<AbstractCICDProvider> logger, IRepositoryMatcher matcher, IMemoryCache cache)
        {
            this.logger = logger;
            this.matcher = matcher;
            this.cache = cache;
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
