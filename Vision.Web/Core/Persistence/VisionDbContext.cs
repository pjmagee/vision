namespace Vision.Web.Core
{
    using Microsoft.EntityFrameworkCore;

    public class VisionDbContext : DbContext
    {
        public DbSet<Squad> Squads { get; set; }

        public DbSet<Ecosystem> Ecosystems { get; set; }
        public DbSet<EcosystemVersion> EcosystemVersions { get; set; }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetDependency> AssetDependencies { get; set; }
        public DbSet<AssetEcosystem> AssetEcoSystems { get; set; }

        public DbSet<Dependency> Dependencies { get; set; }
        public DbSet<DependencyVersion> DependencyVersions { get; set; }

        public DbSet<VulnerabilityReport> VulnerabilityReports { get; set; }

        public DbSet<Vcs> VcsSources { get; set; }
        public DbSet<VcsRepository> VcsRepositories { get; set; }

        public DbSet<CiCd> CicdSources { get; set; }
        public DbSet<EcoRegistry> EcoRegistrySources { get; set; }
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

            modelBuilder.Entity<EcoRegistry>((entity) =>
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
                entity.HasOne(x => x.AssetEcosystem).WithOne(x => x.Asset).HasForeignKey<AssetEcosystem>(x => x.AssetId);
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

            modelBuilder.Entity<VulnerabilityReport>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Link).IsRequired();
                entity.Property(x => x.ResponseData).IsRequired();
            });

            modelBuilder.Entity<DependencyVersion>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Version).IsRequired();

                entity.HasMany(x => x.Vulnerabilities)
                        .WithOne(x => x.DependencyVersion)
                        .HasForeignKey(x => x.DependencyVersionId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ecosystem>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired();
                entity.HasMany(x => x.EcosystemVersions)
                        .WithOne(x => x.Ecosystem)
                        .HasForeignKey(x => x.EcosystemId)
                        .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EcosystemVersion>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.EcosystemId).IsRequired();
                entity.Property(x => x.Version);
            });

            modelBuilder.Entity<CiCd>((entity) =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Endpoint).IsRequired();
            });

            modelBuilder.Entity<Vcs>().HasData(Vcs.MockData());
            modelBuilder.Entity<EcoRegistry>().HasData(EcoRegistry.MockData());
            modelBuilder.Entity<CiCd>().HasData(CiCd.MockData());
        }
    }
}
