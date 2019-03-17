﻿namespace Vision.Web.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Logging;

    public class RegistryService : IRegistryService
    {
        private readonly IDataProtector protector;
        private readonly VisionDbContext context;
        private readonly ILogger<RegistryService> logger;

        public RegistryService(VisionDbContext context, IDataProtectionProvider provider, ILogger<RegistryService> logger)
        {
            this.protector = provider.CreateProtector("Registry");
            this.context = context;
            this.logger = logger;
        }

        public async Task<RegistryDto> CreateAsync(RegistryDto dto)
        {
            Registry entity = new Registry
            {
                Id = Guid.NewGuid(),
                Endpoint = dto.Endpoint,
                Kind = dto.Kind,
                IsPublic = dto.IsPublic,
                IsEnabled = dto.IsEnabled,
                ApiKey = string.IsNullOrWhiteSpace(dto.ApiKey) ? null : protector.Protect(dto.ApiKey),
                Username = string.IsNullOrWhiteSpace(dto.Username) ? null : protector.Protect(dto.Username),
                Password = string.IsNullOrWhiteSpace(dto.Password) ? null : protector.Protect(dto.Password)
            };

            context.Registries.Add(entity);
            await context.SaveChangesAsync();

            logger.LogInformation("Created registry {0}", entity.Id);

            return new RegistryDto { Endpoint = entity.Endpoint, ApiKey = entity.ApiKey, Kind = entity.Kind, RegistryId = entity.Id, IsPublic = dto.IsPublic, IsEnabled = dto.IsEnabled };
        }

        public async Task<RegistryDto> UpdateAsync(RegistryDto dto)
        {
            Registry entity = new Registry
            {
                Id = dto.RegistryId,
                Endpoint = dto.Endpoint,
                Kind = dto.Kind,
                IsPublic = dto.IsPublic,
                IsEnabled = dto.IsEnabled,
                ApiKey = string.IsNullOrWhiteSpace(dto.ApiKey) ? null : protector.Protect(dto.ApiKey),
                Username = string.IsNullOrWhiteSpace(dto.Username) ? null : protector.Protect(dto.Username),
                Password = string.IsNullOrWhiteSpace(dto.Password) ? null : protector.Protect(dto.Password)
            };

            context.Registries.Update(entity);
            await context.SaveChangesAsync();

            logger.LogInformation("Updated registry {0}", entity.Id);

            return new RegistryDto { Endpoint = entity.Endpoint, ApiKey = entity.ApiKey, Kind = entity.Kind, RegistryId = entity.Id, IsPublic = dto.IsPublic, IsEnabled = dto.IsEnabled };
        }

        public async Task<IPaginatedList<RegistryDto>> GetAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = context.Registries.Select(registry => new RegistryDto
            {
                ApiKey = string.IsNullOrWhiteSpace(registry.ApiKey) ? null : protector.Unprotect(registry.ApiKey),
                Username = string.IsNullOrWhiteSpace(registry.Username) ? null : protector.Unprotect(registry.Username),
                Password = string.IsNullOrWhiteSpace(registry.Password) ? null : protector.Unprotect(registry.Password),
                Dependencies = context.Dependencies.Count(d => d.RegistryId == registry.Id),
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
            Registry entity = await context.Registries.FindAsync(registryId);

            return new RegistryDto
            {
                ApiKey = string.IsNullOrWhiteSpace(entity.ApiKey) ? null : protector.Unprotect(entity.ApiKey),
                Username = string.IsNullOrWhiteSpace(entity.Username) ? null : protector.Unprotect(entity.Username),
                Password = string.IsNullOrWhiteSpace(entity.Password) ? null : protector.Unprotect(entity.Password),
                Dependencies = context.Dependencies.Count(d => d.RegistryId == entity.Id),
                Endpoint = entity.Endpoint,
                IsEnabled = entity.IsEnabled,
                IsPublic = entity.IsPublic,
                Kind = entity.Kind,
                RegistryId = entity.Id
            };
        }
    }
}