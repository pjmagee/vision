namespace Vision.Web.Core
{
    using Microsoft.EntityFrameworkCore;

    public class VisionDbContext : DbContext
    {
        public DbSet<Squad> Squads { get; set; }

        public DbSet<Runtime> Runtimes { get; set; }
        public DbSet<RuntimeVersion> RuntimeVersions { get; set; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetDependency> AssetDependencies { get; set; }
        public DbSet<AssetRuntime> AssetRuntimes { get; set; }


        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<DependencyVersion> DependencyVersions { get; set; }

        public DbSet<Vcs> VersionControls { get; set; }
        public DbSet<VcsRepository> Repositories { get; set; }

        public DbSet<CiCd> CiCds { get; set; }
        public DbSet<ArtifactRegistry> Registries { get; set; }
        public DbSet<RefreshTask> Tasks { get; set; }

        public VisionDbContext(DbContextOptions<VisionDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // https://docs.microsoft.com/en-us/ef/core/querying/related-data
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshTask>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Created).IsRequired();
                entity.Property(x => x.Scope).IsRequired();
            });

            modelBuilder.Entity<Vcs>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ApiKey).IsRequired();
                entity.Property(x => x.Kind).IsRequired();
                entity.HasMany(x => x.Repositories).WithOne(r => r.Vcs).HasForeignKey(s => s.VcsId);
            });

            modelBuilder.Entity<VcsRepository>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Url).IsRequired();
                entity.Property(x => x.WebUrl).IsRequired();
            });

            modelBuilder.Entity<ArtifactRegistry>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Endpoint).IsRequired();
                entity.Property(x => x.Kind).IsRequired();
                entity.Property(x => x.ApiKey);
                entity.Property(x => x.Username);
                entity.Property(x => x.Password);
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Path).IsRequired();
                entity.Property(x => x.Raw).IsRequired();
                entity.HasOne(x => x.AssetRuntime).WithOne(x => x.Asset).HasForeignKey<AssetRuntime>(x => x.AssetId);
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
                entity.Property(e => e.Kind);
                entity.HasMany(x => x.Versions).WithOne(v => v.Dependency).HasForeignKey(x => x.DependencyId);
                entity.HasMany(x => x.Assets).WithOne(x => x.Dependency).HasForeignKey(x => x.DependencyId);
            });

            modelBuilder.Entity<DependencyVersion>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Version).IsRequired();
            });

            modelBuilder.Entity<Runtime>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired();
                entity.HasMany(x => x.Versions).WithOne(x => x.Runtime).HasForeignKey(x => x.RuntimeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RuntimeVersion>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.RuntimeId).IsRequired();
                entity.Property(x => x.Version);
            });

            modelBuilder.Entity<CiCd>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Endpoint).IsRequired();
            });

            modelBuilder.Entity<Vcs>().HasData(Vcs.MockData());
            modelBuilder.Entity<ArtifactRegistry>().HasData(ArtifactRegistry.MockData());
            modelBuilder.Entity<CiCd>().HasData(CiCd.MockData());
        }
    }
}
