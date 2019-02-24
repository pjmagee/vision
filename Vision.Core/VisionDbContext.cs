﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vision.Shared;

namespace Vision.Core
{
    public class VisionDbContext : DbContext
    {
        public VisionDbContext(DbContextOptions<VisionDbContext> options) : base(options) { }

        public DbSet<Squad> Squads { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<Framework> Frameworks { get; set; }
        public DbSet<DependencyVersion> DependencyVersions { get; set; }
        public DbSet<AssetDependency> AssetDependencies { get; set; }
        public DbSet<AssetFramework> AssetFrameworks { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<VersionControl> VersionControls { get; set; }
        public DbSet<CiCd> CiCds { get; set; }
        public DbSet<Registry> Registries { get; set; }
        public DbSet<SystemTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // https://docs.microsoft.com/en-us/ef/core/querying/related-data
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemTask>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.Created).IsRequired();
                entity.Property(x => x.Scope).IsRequired();
                entity.Property(x => x.Completed);
            });

            modelBuilder.Entity<Repository>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Url).IsRequired();
                entity.Property(x => x.WebUrl).IsRequired();                
            });

            modelBuilder.Entity<VersionControl>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ApiKey).IsRequired();
                entity.Property(x => x.Kind).IsRequired();
                entity.HasMany(x => x.Repositories).WithOne(r => r.VersionControl).HasForeignKey(s => s.VersionControlId);
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Path).IsRequired();
                entity.Property(x => x.Raw).IsRequired();
                entity.HasMany(x => x.Dependencies).WithOne(x => x.Asset).HasForeignKey(x => x.AssetId);
            });

            modelBuilder.Entity<AssetDependency>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Dependency).WithMany(x => x.Assets).HasForeignKey(x => x.DependencyId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.DependencyVersion).WithMany(x => x.Assets).HasForeignKey(x => x.DependencyVersionId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Dependency>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Kind).HasConversion(new EnumToStringConverter<DependencyKind>());
                entity.HasMany(x => x.Versions).WithOne(v => v.Dependency).HasForeignKey(x => x.DependencyId);
                entity.HasMany(x => x.Assets).WithOne(x => x.Dependency).HasForeignKey(x => x.DependencyId);
            });

            modelBuilder.Entity<DependencyVersion>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Version).IsRequired();                  
            });

            modelBuilder.Entity<Framework>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Version).IsRequired();
                entity.HasMany(x => x.Frameworks).WithOne(x => x.Framework).HasForeignKey(x => x.FrameworkId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
