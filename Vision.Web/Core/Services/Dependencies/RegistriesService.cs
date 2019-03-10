namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Vision.Web.Core;

    public class RegistriesService
    {
        private readonly IDataProtector protector;
        private readonly VisionDbContext context;

        public RegistriesService(VisionDbContext context, IDataProtectionProvider providoer)
        {
            this.protector = providoer.CreateProtector("Registry");
            this.context = context;
        }

        public async Task<ActionResult<RegistryDto>> CreateRegistryAsync(RegistryDto registryDto)
        {
            Registry registry = new Registry
            {
                Id = Guid.NewGuid(),
                ApiKey = protector.Protect(registryDto.ApiKey),
                Endpoint = registryDto.Endpoint,
                Kind = registryDto.Kind,
                IsPublic = registryDto.IsPublic,
                Username = protector.Protect(registryDto.Username),
                Password = protector.Protect(registryDto.Password)
            };

            context.Registries.Add(registry);
            await context.SaveChangesAsync();

            return new RegistryDto { Endpoint = registry.Endpoint, ApiKey = registry.ApiKey, Kind = registry.Kind, RegistryId = registry.Id, IsPublic = registryDto.IsPublic };
        }

        public async Task<IEnumerable<RegistryDto>> GetAllAsync()
        {
            return await context.Registries.Select(registry => new RegistryDto
            {
                ApiKey = protector.Unprotect(registry.ApiKey),
                Password = protector.Unprotect(registry.Password),
                Username = protector.Unprotect(registry.Username),
                Dependencies = context.Dependencies.Count(d => d.RegistryId == registry.Id),
                Endpoint = registry.Endpoint,
                Kind = registry.Kind,
                RegistryId = registry.Id                
            })
            .ToListAsync();
        }

        public async Task<RegistryDto> GetRegistryByIdAsync(Guid registryId)
        {
            Registry registry = await context.Registries.FindAsync(registryId);

            return new RegistryDto
            {
                ApiKey = protector.Unprotect(registry.ApiKey),
                Password = protector.Unprotect(registry.Password),
                Username = protector.Unprotect(registry.Username),
                Dependencies = context.Dependencies.Count(d => d.RegistryId == registry.Id),
                Endpoint = registry.Endpoint,
                Kind = registry.Kind,
                RegistryId = registry.Id
            };
        }

        public async Task<IEnumerable<DependencyDto>> GetDependenciesByRegistryIdAsync(Guid registryId)
        {
            return await context.Dependencies
                .Where(d => d.RegistryId == registryId)
                .Select(d => new DependencyDto
                {
                    Name = d.Name,
                    RepositoryUrl = d.RepositoryUrl,
                    Versions = context.DependencyVersions.Count(x => x.DependencyId == d.Id),
                    Assets = context.AssetDependencies.Count(x => x.DependencyId == d.Id),
                    DependencyId = d.Id,
                    Kind = d.Kind
                }).ToListAsync();
        }
    }
}
