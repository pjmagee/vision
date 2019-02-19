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
        public DbSet<GitRepository> GitRepositories { get; set; }
        public DbSet<GitSource> GitSources { get; set; }
        public DbSet<BuildSources> BuildSources { get; set; }
        public DbSet<Registry> Registries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // https://docs.microsoft.com/en-us/ef/core/querying/related-data
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GitRepository>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.GitUrl).IsRequired();
                entity.Property(x => x.WebUrl).IsRequired();                
            });

            modelBuilder.Entity<GitSource>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ApiKey).IsRequired();
                entity.Property(x => x.Kind).IsRequired();
                entity.HasMany(x => x.GitRepositories).WithOne(r => r.GitSource).HasForeignKey(s => s.GitSourceId);
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
            });

            modelBuilder.Entity<Dependency>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(e => e.Kind).HasConversion(new EnumToStringConverter<DependencyKind>());
                entity.HasMany(x => x.Versions).WithOne(v => v.Dependency).HasForeignKey(x => x.DependencyId);
            });

            modelBuilder.Entity<DependencyVersion>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Version).IsRequired();
                entity.Property(x => x.IsVulnerable).HasDefaultValue(false);    
            });
        }
    }
}
