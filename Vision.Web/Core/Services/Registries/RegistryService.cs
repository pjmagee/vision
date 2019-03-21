namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class RegistryService : IRegistryService
    {
        private readonly IEncryptionService encryptionService;
        private readonly VisionDbContext context;
        private readonly ILogger<RegistryService> logger;

        public RegistryService(VisionDbContext context, IEncryptionService encryptionService, ILogger<RegistryService> logger)
        {
            this.encryptionService = encryptionService;
            this.context = context;
            this.logger = logger;
        }

        public async Task<RegistryDto> CreateAsync(RegistryDto dto)
        {
            Registry registry = new Registry
            {
                Endpoint = dto.Endpoint,
                Kind = dto.Kind,
                IsPublic = dto.IsPublic,
                IsEnabled = dto.IsEnabled,
                ApiKey = encryptionService.Encrypt(dto.ApiKey),
                Username = encryptionService.Encrypt(dto.Username),
                Password = encryptionService.Encrypt(dto.Password),
            };

            context.Registries.Add(registry);
            await context.SaveChangesAsync();

            logger.LogInformation("Created registry {0}", registry.Id);

            return new RegistryDto { Endpoint = registry.Endpoint, ApiKey = registry.ApiKey, Kind = registry.Kind, RegistryId = registry.Id, IsPublic = dto.IsPublic, IsEnabled = dto.IsEnabled };
        }

        public async Task<RegistryDto> UpdateAsync(RegistryDto dto)
        {
            var registry = context.Registries.Find(dto.RegistryId);

            registry.IsEnabled = dto.IsEnabled;
            registry.IsPublic = dto.IsPublic;
            registry.Kind = dto.Kind;
            registry.Endpoint = dto.Endpoint;

            if (registry.ApiKey != dto.ApiKey)
                registry.ApiKey = encryptionService.Encrypt(dto.ApiKey);

            if (registry.Username != dto.Username)
                registry.Username = encryptionService.Encrypt(dto.Username);

            if (registry.Password != dto.Password)
                registry.Password = encryptionService.Encrypt(dto.Password);

            context.Registries.Update(registry);
            await context.SaveChangesAsync();

            logger.LogInformation("Updated registry {0}", registry.Id);

            return new RegistryDto { Endpoint = registry.Endpoint, ApiKey = registry.ApiKey, Kind = registry.Kind, RegistryId = registry.Id, IsPublic = dto.IsPublic, IsEnabled = dto.IsEnabled };
        }

        public async Task<IPaginatedList<RegistryDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Registries.Select(registry => new RegistryDto
            {
                ApiKey = encryptionService.Decrypt(registry.ApiKey),
                Username = encryptionService.Decrypt(registry.Username),
                Password = encryptionService.Decrypt(registry.Password),                
                Endpoint = registry.Endpoint,
                IsEnabled = registry.IsEnabled,
                IsPublic = registry.IsPublic,
                Kind = registry.Kind,
                RegistryId = registry.Id
            });

            return await PaginatedList<RegistryDto>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<RegistryDto> GetByIdAsync(Guid registryId)
        {
            Registry registry = await context.Registries.FindAsync(registryId);

            return new RegistryDto
            {
                ApiKey = encryptionService.Decrypt(registry.ApiKey),
                Username = encryptionService.Decrypt(registry.Username),
                Password = encryptionService.Decrypt(registry.Password),                
                Endpoint = registry.Endpoint,
                IsEnabled = registry.IsEnabled,
                IsPublic = registry.IsPublic,
                Kind = registry.Kind,
                RegistryId = registry.Id
            };
        }

        public async Task<List<RegistryDto>> GetEnabledByKindAsync(DependencyKind kind)
        {
            return await context.Registries
                .Where(registry => registry.Kind == kind && registry.IsEnabled)
                .OrderBy(registry => !registry.IsPublic)
                .Select(registry => new RegistryDto
                {
                    ApiKey = encryptionService.Decrypt(registry.ApiKey),
                    Username = encryptionService.Decrypt(registry.Username),
                    Password = encryptionService.Decrypt(registry.Password),                    
                    Endpoint = registry.Endpoint,
                    IsEnabled = registry.IsEnabled,
                    IsPublic = registry.IsPublic,
                    Kind = registry.Kind,
                    RegistryId = registry.Id
                })
                .ToListAsync();
        }
    }
}
