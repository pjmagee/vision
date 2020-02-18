using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace Vision.Core
{
    public class FakeDataGenerator
    {
        private readonly VisionDbContext context;
        private readonly IEncryptionService encryptionService;

        private const string FakeResponseData =
@"
{
  ""coordinates"": ""pkg:nuget/ClosedXML@0.7.6"",
  ""description"": ""ClosedXML makes it easier for developers to create Excel 2007/2010 files. It provides a nice object oriented way to manipulate the files (similar to VBA) without dealing with the hassles of XML Documents. It can be used by any .NET language like C# and Visual Basic (VB)."",
  ""reference"": ""https://ossindex.sonatype.org/component/pkg:nuget/ClosedXML@0.7.6"",
  ""vulnerabilities"":
    [{
      ""id"": ""77022a1d-79ce-4018-a60e-f1db56fc81ef"",
      ""title"": ""OS Command Injection"",
      ""description"": ""Flaws in two methods in PathHelper contain OS command injection flaws.\n\nThe public methods were not used in any part of the library or its examples."",
      ""cvssScore"": 0,
      ""reference"": ""https://ossindex.sonatype.org/vuln/77022a1d-79ce-4018-a60e-f1db56fc81ef""
    }]
}";

        private readonly static IList<EcosystemKind> EcosystemKinds = Enum.GetValues(typeof(EcosystemKind)).Cast<EcosystemKind>().ToList();

        public FakeDataGenerator(VisionDbContext context, IEncryptionService encryptionService)
        {
            this.encryptionService = encryptionService;
            this.context = context;
        }

        public async Task SeedAsync()
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await PopulateVersionControls();
            await PopulateVcsRepositories();
            await PopulateArtifactRegistries();
            await PopulateCiCds();

            await PopulateEcoSystems();

            await PopulateDependencies();
            await PopulateDependencyVersions();
            await PopulateDependencyVersionVulnerabilities();

            await PopulateAssets();
            await PopulateAssetDependencies();
            await PopulateAssetEcosystems();
        }

        private async Task PopulateDependencyVersionVulnerabilities()
        {
            foreach (Dependency dependency in context.Dependencies)
            {
                UniqueRandomPicker<DependencyVersion> picker = Pick<DependencyVersion>.UniqueRandomList(With.Between(0).And(dependency.Versions.Count).Elements);

                foreach (DependencyVersion version in picker.From(dependency.Versions.ToList()))
                {
                    context.Vulnerabilities.Add(new Vulnerability
                    {
                        DependencyVersion = version,
                        DependencyVersionId = version.Id,
                        Id = Guid.NewGuid(),
                        CheckTime = DateTime.Now,
                        Kind = ReporterKind.OSSIndex,
                        ResponseData = FakeResponseData,
                        Link = $"https://ossindex.sonatype.org/component/pkg:nuget/ClosedXML@0.7.6"
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task PopulateAssetEcosystems()
        {
            foreach (Asset asset in context.Assets)
            {
                foreach (Ecosystem ecosystem in context.Ecosystems)
                {
                    if (OnRuntime(asset, ecosystem))
                    {
                        EcosystemVersion ecosystemVersion = Pick<EcosystemVersion>.RandomItemFrom(ecosystem.EcosystemVersions.ToList());
                        AssetEcosystem assetEcoSystem = GetAssetEcosystem(asset, ecosystemVersion);
                        context.AssetEcoSystems.Add(assetEcoSystem);
                        break;
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task PopulateAssetDependencies()
        {
            foreach (Asset asset in context.Assets)
            {
                var assetDependencies = GetAssetDependencies(asset, context.Dependencies.Where(d => d.Kind == asset.Kind).ToList());
                context.AssetDependencies.AddRange(assetDependencies);
            }

            await context.SaveChangesAsync();
        }

        private async Task PopulateDependencyVersions()
        {
            foreach (Dependency dependency in context.Dependencies)
            {
                context.DependencyVersions.AddRange(GetDependencyVersions(dependency));
            }

            await context.SaveChangesAsync();
        }

        private async Task PopulateDependencies()
        {
            List<VcsRepository> repositories = context.VcsRepositories.ToList();
            List<EcoRegistry> registries = context.EcoRegistrySources.ToList();

            foreach (EcosystemKind kind in EcosystemKinds)
            {
                context.Dependencies.AddRange(GetDependencies(registries, repositories, kind));
            }

            await context.SaveChangesAsync();
        }

        private async Task PopulateAssets()
        {
            foreach (VcsRepository repository in context.VcsRepositories)
            {
                context.Assets.AddRange(GetAssets(repository));
            }

            await context.SaveChangesAsync();
        }

        private async Task PopulateVersionControls()
        {
            context.VcsSources.AddRange(GetVersionControls());
            await context.SaveChangesAsync();
        }

        private async Task PopulateCiCds()
        {
            context.CicdSources.AddRange(GetCiCds());
            await context.SaveChangesAsync();
        }

        private async Task PopulateArtifactRegistries()
        {
            context.EcoRegistrySources.AddRange(GetRegistries());
            await context.SaveChangesAsync();
        }

        private async Task PopulateEcoSystems()
        {
            context.Ecosystems.AddRange(GetEcosystems());
            await context.SaveChangesAsync();

            foreach (Ecosystem ecosystem in context.Ecosystems)
            {
                context.EcosystemVersions.AddRange(GetEcoVersions(ecosystem));
            }
            await context.SaveChangesAsync();
        }

        private async Task PopulateVcsRepositories()
        {
            foreach (Vcs vcs in context.VcsSources)
            {
                context.VcsRepositories.AddRange(GetRepositories(vcs));
            }
            await context.SaveChangesAsync();
        }

        private static int GetAssetsByKind(EcosystemKind kind) => kind switch
        {
            EcosystemKind.Docker => GetRandom.Int(1, 2),
            EcosystemKind.NuGet => GetRandom.Int(1, 5),
            EcosystemKind.Maven => GetRandom.Int(1, 5),
            EcosystemKind.Gradle => GetRandom.Int(1, 5),
            EcosystemKind.Npm => GetRandom.Int(1, 2),
            EcosystemKind.PyPi => GetRandom.Int(1, 5),
            EcosystemKind.RubyGem => GetRandom.Int(1, 5),
            _ => throw new Exception("Unhandled kind for number of assets")
        };

        private static int GetDependenciesForAsset(EcosystemKind kind) => kind switch
        {
            EcosystemKind.Docker => GetRandom.Int(1, 3),
            EcosystemKind.NuGet => GetRandom.Int(2, 6),
            EcosystemKind.Maven => GetRandom.Int(2, 6),
            EcosystemKind.Gradle => GetRandom.Int(2, 5),
            EcosystemKind.Npm => GetRandom.Int(2, 10),
            EcosystemKind.RubyGem => GetRandom.Int(2, 10),
            EcosystemKind.PyPi => GetRandom.Int(2, 6),
            _ => throw new Exception("Unhandled kind for number of dependencies")
        };

        private IEnumerable<Asset> GetAssets(VcsRepository repository)
        {
            string[] items = repository.WebUrl.Split('/');
            string fileOrFolderName = items[items.Length - 2];
            IList<EcosystemKind> kindsOfAssetsInGitRepository = Pick<EcosystemKind>.UniqueRandomList(With.Between(1).And(EcosystemKinds.Count).Elements).From(EcosystemKinds);

            return kindsOfAssetsInGitRepository.SelectMany(kind => Builder<Asset>.CreateListOfSize(GetAssetsByKind(kind)).All()
                .With(a => a.Id = Guid.NewGuid())
                .With(a => a.Raw = $"FILE CONTENTS FOR PACKAGE KIND OF {kind}")
                .With(a => a.Kind = kind)
                .With((a, fileIndex) => a.Path = GetPathForAsset(fileOrFolderName, fileIndex, kind))
                .With(a => a.Repository = repository)
                .With(a => a.RepositoryId = repository.Id)
                .Build());
        }

        private IEnumerable<Dependency> GetDependencies(IList<EcoRegistry> registries, IList<VcsRepository> repositories, EcosystemKind kind)
        {
            // var privatePicker = new RandomItemPicker<Repository>(repositories, new RandomGenerator());
            // var publicPicker = new RandomItemPicker<string>(new[] { "http://github.com", "http://bitbucket.com", "http://gitlab.com" }, new RandomGenerator());

            var names = Enumerable.Range(0, GetRandomDependencies(kind)).Select(x => GetGeneratedNameByKind(kind)).Distinct().ToList();
            var namePicker = new RandomItemPicker<string>(names, new UniqueRandomGenerator());
            RandomItemPicker<EcoRegistry> registryPicker = GetRegistryByKind(registries, kind);

            return Builder<Dependency>.CreateListOfSize(names.Count)
                .All()
                .With(d => d.Id = Guid.NewGuid())
                .With(d => d.Name = namePicker.Pick().Trim())
                .With(d => d.Kind = kind)
                .With(d => d.Updated = DateTime.Now.Add(new TimeSpan(GetRandom.Int(-150, 0), GetRandom.Int(0, 10), GetRandom.Int(0, 10), GetRandom.Int(0, 10), GetRandom.Int(0, 500))))
                .Build()
                .Distinct();
        }

        private AssetEcosystem GetAssetEcosystem(Asset asset, EcosystemVersion runtimeVersion)
        {
            return Builder<AssetEcosystem>
                .CreateNew()
                .With(ar => ar.Id = Guid.NewGuid())
                .With(ar => ar.Asset = asset).With(ar => ar.AssetId = asset.Id)
                .With(ar => ar.EcosystemVersion = runtimeVersion).With(ar => ar.EcosystemVersionId = runtimeVersion.Id)
                .Build();
        }

        private bool OnRuntime(Asset asset, Ecosystem runtime)
        {
            if (asset.Kind == EcosystemKind.NuGet && runtime.Name.Contains(".NET")) return true;
            if (asset.Kind == EcosystemKind.Npm && runtime.Name.Equals("Node")) return true;
            if ((asset.Kind == EcosystemKind.Maven || asset.Kind == EcosystemKind.Gradle) && runtime.Name.Equals("Java")) return true;
            return false;
        }

        private IEnumerable<AssetDependency> GetAssetDependencies(Asset asset, IList<Dependency> dependencies)
        {
            var max = Math.Min(dependencies.Count, GetDependenciesForAsset(asset.Kind));
            var selectedDependencies = Pick<Dependency>.UniqueRandomList(With.Between(1).And(max).Elements).From(dependencies);

            return selectedDependencies.Select(dependency =>
                Builder<AssetDependency>
                    .CreateNew()
                    .With(ad => ad.Id = Guid.NewGuid())
                    .With(ad => ad.AssetId = asset.Id)
                    .With(ad => ad.Asset = asset)
                    .And((ad) =>
                    {
                        ad.Dependency = dependency;
                        ad.DependencyId = dependency.Id;

                        DependencyVersion pickedVersion = Pick<DependencyVersion>.RandomItemFrom(dependency.Versions.ToList());

                        ad.DependencyVersion = pickedVersion;
                        ad.DependencyVersionId = pickedVersion.Id;

                    }).Build()
            );

        }

        private IEnumerable<DependencyVersion> GetDependencyVersions(Dependency dependency)
        {
            var versionCount = GetRandom.Int(1, 15);

            var versions = Builder<DependencyVersion>.CreateListOfSize(versionCount)
                .All()
                .With(dv => dv.IsLatest = false)
                .With(dv => dv.Id = Guid.NewGuid())
                .With(dv => dv.Dependency = dependency)
                .With(dv => dv.DependencyId = dependency.Id)
                .With((dv, index) => dv.Version = new Version(GetRandom.Int(index, versionCount), GetRandom.Int(index, versionCount), GetRandom.Int(index, versionCount)).ToString())
                .Build().ToList();

            versions.Sort(new VersionComparer());

            versions[versions.Count - 1].IsLatest = true;

            return versions;
        }

        private IEnumerable<Vcs> GetVersionControls() => new Vcs[]
        {
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Bitbucket).With(vc => vc.Endpoint = $"https://{vc.Kind}.market1.company.com:8080/").Build(),
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Bitbucket).With(vc => vc.Endpoint = $"https://{vc.Kind}.market2.company.com:8080/").Build(),
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Gitlab).With(vc    => vc.Endpoint = $"https://{vc.Kind}.market3.company.com:8080/").Build(),
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Bitbucket).With(vc => vc.Endpoint = $"https://{vc.Kind}.market4.company.com:8080/").Build(),
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Gitlab).With(vc    => vc.Endpoint = $"https://{vc.Kind}.market5.company.com:8080/").Build(),
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Gitlab).With(vc    => vc.Endpoint = $"https://{vc.Kind}.market6.company.com:8080/").Build(),
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Bitbucket).With(vc => vc.Endpoint = $"https://{vc.Kind}.market7.company.com:8080/").Build(),
            CreateNewVersionControl().With(x => x.Kind = VcsKind.Gitlab).With(vc    => vc.Endpoint = $"https://{vc.Kind}.shared.company.com:8080/").Build(),
        };

        private IEnumerable<CiCd> GetCiCds() => new CiCd[]
        {
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.market1.company.com:8080/").With(x => x.Kind = CiCdKind.Jenkins).Build(),
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.market2.company.com:8080/").With(x => x.Kind = CiCdKind.Jenkins).Build(),
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.market3.company.com:8080/").With(x => x.Kind = CiCdKind.TeamCity).Build(),
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.market4.company.com:8080/").With(x => x.Kind = CiCdKind.TeamCity).Build(),
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.market5.company.com:8080/").With(x => x.Kind = CiCdKind.Gitlab).Build(),
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.market6.company.com:8080/").With(x => x.Kind = CiCdKind.Gitlab).Build(),
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.market7.company.com:8080/").With(x => x.Kind = CiCdKind.TeamCity).Build(),
            CreateNewCiCd().With(ci => ci.Endpoint = $"https://{ci.Kind}.shared.company.com:8080/").With(x => x.Kind = CiCdKind.Gitlab).Build()
        };

        private ISingleObjectBuilder<Vcs> CreateNewVersionControl() => Builder<Vcs>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.ApiKey = Guid.NewGuid().ToString());

        private ISingleObjectBuilder<Ecosystem> CreateNewEcoSystem() => Builder<Ecosystem>.CreateNew().With(x => x.Id = Guid.NewGuid());

        private ISingleObjectBuilder<CiCd> CreateNewCiCd() => Builder<CiCd>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.ApiKey = encryptionService.Encrypt(Guid.NewGuid().ToString()))
            .With(x => x.Username = encryptionService.Encrypt("Username"))
            .With(x => x.Password = encryptionService.Encrypt("Password"));

        private ISingleObjectBuilder<EcoRegistry> CreateNewRegistry() => Builder<EcoRegistry>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.ApiKey = encryptionService.Encrypt(Guid.NewGuid().ToString()))
            .With(x => x.Username = encryptionService.Encrypt("Username"))
            .With(x => x.Password = encryptionService.Encrypt("Password"));

        private IEnumerable<EcoRegistry> GetRegistries() => EcosystemKinds.SelectMany(kind => new EcoRegistry[]
        {
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market1.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market2.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market3.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market4.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market5.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market6.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market7.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market8.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market9.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market10.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market11.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market12.company.com/{x.Kind}/".ToLower()).Build(),
            CreateNewRegistry().With(x => x.IsPublic = false).With(x => x.Kind = kind).With(x => x.Endpoint = $"https://nexus.market13.company.com/{x.Kind}/".ToLower()).Build(),
        })
        .Concat(new[]
        {
            CreateNewRegistry().With(x => x.Endpoint = $"https://hub.docker.com/")        .With(x => x.IsPublic = true).With(x => x.Kind = EcosystemKind.Docker).Build(),
            CreateNewRegistry().With(x => x.Endpoint = $"https://registry.npm.com/")      .With(x => x.IsPublic = true).With(x => x.Kind = EcosystemKind.Npm).Build(),
            CreateNewRegistry().With(x => x.Endpoint = $"https://nuget.org.com/v3/")      .With(x => x.IsPublic = true).With(x => x.Kind = EcosystemKind.NuGet).Build(),
            CreateNewRegistry().With(x => x.Endpoint = $"https://registry.maven.com/")    .With(x => x.IsPublic = true).With(x => x.Kind = EcosystemKind.Maven).Build(),
            CreateNewRegistry().With(x => x.Endpoint = $"https://registry.maven.com/")    .With(x => x.IsPublic = true).With(x => x.Kind = EcosystemKind.Gradle).Build(),
            CreateNewRegistry().With(x => x.Endpoint = $"https://registry.python.com/")   .With(x => x.IsPublic = true).With(x => x.Kind = EcosystemKind.PyPi).Build(),
            CreateNewRegistry().With(x => x.Endpoint = $"https://registry.rubygems.com/") .With(x => x.IsPublic = true).With(x => x.Kind = EcosystemKind.RubyGem).Build()
        });

        private IEnumerable<Ecosystem> GetEcosystems()
        {
            yield return CreateNewEcoSystem().With(x => x.Name = ".NET").Build();
            yield return CreateNewEcoSystem().With(x => x.Name = "Java").Build();
            yield return CreateNewEcoSystem().With(x => x.Name = "Ruby").Build();
            yield return CreateNewEcoSystem().With(x => x.Name = "Node").Build();
            // yield return Builder<Runtime>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Name = ".NET Standard").Build();
            // yield return Builder<Runtime>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Name = ".NET Core").Build();
            // yield return Builder<Runtime>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Name = ".NET Framework").Build();
            // yield return Builder<Runtime>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Name = "Python").Build();
        }

        public IEnumerable<EcosystemVersion> GetEcoVersions(Ecosystem runtime)
        {
            var netFrameworks = new[] { "net11", "net20", "net35", "net40", "net403", "net45", "net451", "net452", "net46", "net461" };
            var netStandards = new[] { "netstandard1.0", "netstandard1.1", "netstandard1.2", "netstandard1.3", "netstandard1.4", "netstandard1.5", "netstandard1.6", "netstandard2.0", "netstandard2.1" };
            var netCores = new[] { "netcoreapp1.0", "netcoreapp1.1", "netcoreapp2.0", "netcoreapp2.1", "netcoreapp2.2", "netcoreapp3.0", "netcoreapp3.1" };

            return runtime.Name switch
            {
                // ".NET Framework" => netFrameworks.Select(f => Builder<RuntimeVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Runtime = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = f).Build()),
                // ".NET Standard" => netStandards.Select(f => Builder<RuntimeVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Runtime = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = f).Build()),
                // ".NET Core" => netCores.Select(f => Builder<RuntimeVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Runtime = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = f).Build()),
                ".NET" => netCores.Concat(netFrameworks).Concat(netStandards).Select(f => Builder<EcosystemVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Ecosystem = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = f).Build()),
                "Java" => new[] { "6", "7", "8", "9", "10", "11", "12", "13" }.Select(f => Builder<EcosystemVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Ecosystem = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = f).Build()),
                "Ruby" => new[] { "2.7.0", "2.4.9", "2.4.5", "2.4.1", "2.2.1", "2.1.1", "1.9.2" }.Select(f => Builder<EcosystemVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Ecosystem = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = f).Build()),
                "Node" => Enumerable.Range(4, 11).Select(x => new Version(x, GetRandom.Int(1, 12), GetRandom.Int(1, 12)).ToString()).Select(f => Builder<EcosystemVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Ecosystem = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = f).Build()),
                _ => new[] { Builder<EcosystemVersion>.CreateNew().With(x => x.Id = Guid.NewGuid()).With(x => x.Ecosystem = runtime).With(x => x.EcosystemId = runtime.Id).With(x => x.Version = "NOT SET").Build() }
            };
        }

        private IEnumerable<VcsRepository> GetRepositories(Vcs source)
        {
            var repoNames = Enumerable.Range(0, GetRandom.Int(5, 20)).Select(x => GetGeneratedNameByKind(EcosystemKind.Npm).Trim('@')).Distinct().ToList();
            var repoPicker = new RandomItemPicker<string>(repoNames, new UniqueRandomGenerator());

            return Builder<VcsRepository>.CreateListOfSize(GetRandom.Int(1, repoNames.Count))
                .All()
                .With(r => r.Id = Guid.NewGuid())
                .With(r => r.IsIgnored = false)
                .With((r, idx) =>
                {
                    var uri = new Uri(source.Endpoint);
                    var group = GetRandom.String(2).ToUpper();
                    var name = repoPicker.Pick();

                    r.Url = $"ssh://{uri.Host}:{uri.Port}/{group}/{name}.git";
                    r.WebUrl = $"http://{uri.Host}:{uri.Port}/{group}/{name}/browse";
                })
                .With(r => r.Vcs = source)
                .With(r => r.VcsId = source.Id)
                .Build();
        }

        private static RandomItemPicker<EcoRegistry> GetRegistryByKind(IEnumerable<EcoRegistry> sources, EcosystemKind kind) => new RandomItemPicker<EcoRegistry>(sources.Where(x => x.Kind == kind).ToList(), new RandomGenerator());

        private static int GetRandomDependencies(EcosystemKind kind) => kind switch
        {
            EcosystemKind.Docker => GetRandom.Int(1, 10),
            EcosystemKind.Maven => GetRandom.Int(10, 30),
            EcosystemKind.Gradle => GetRandom.Int(10, 30),
            EcosystemKind.NuGet => GetRandom.Int(10, 30),
            EcosystemKind.Npm => GetRandom.Int(10, 30),
            EcosystemKind.PyPi => GetRandom.Int(10, 30),
            EcosystemKind.RubyGem => GetRandom.Int(10, 30),
            _ => throw new Exception("Cannot pick random item picker for dependency sources for chosen dependency kind")
        };

        private static string GetPathForAsset(string fileOrFolderName, int fileIndex, EcosystemKind kind)
        {
            string ext = kind.GetFileExtension();
            bool isFirstFile = fileIndex == 0;

            switch (kind)
            {
                case EcosystemKind.Docker: return isFirstFile ? $"docker/src/{ext}" : $"docker/src/{fileIndex}.{ext}";
                case EcosystemKind.RubyGem: return isFirstFile ? $"ruby/src/{fileOrFolderName}/{ext}" : $"ruby/src/{fileOrFolderName}/{fileIndex}/{ext}";
                case EcosystemKind.Maven: return isFirstFile ? $"java/src/{fileOrFolderName}/{fileOrFolderName}.{ext}" : $"java/src/{fileOrFolderName}/{fileOrFolderName}.{fileIndex}/{fileOrFolderName}.{fileIndex}.{ext}";
                case EcosystemKind.Gradle: return isFirstFile ? $"java/src/{fileOrFolderName}/{ext}" : $"java/src/{fileOrFolderName}/{fileOrFolderName}.{fileIndex}/{ext}";
                case EcosystemKind.NuGet: return isFirstFile ? $"dotnet/src/{fileOrFolderName}/{fileOrFolderName}.{ext}" : $"dotnet/src/{fileOrFolderName}/{fileOrFolderName}.{fileIndex}/{fileOrFolderName}.{fileIndex}.{ext}";
                case EcosystemKind.Npm: return isFirstFile ? $"nodejs/src/{fileOrFolderName}/{ext}" : $"nodejs/src/{fileOrFolderName}/{fileIndex}/{ext}";
                case EcosystemKind.PyPi: return isFirstFile ? $"python/src/{fileOrFolderName}/{ext}" : $"python/src/{fileOrFolderName}/{fileIndex}/{ext}";
            }

            throw new Exception("Unhandled kind");
        }

        private static string GetGeneratedNameByKind(EcosystemKind kind)
        {
            string left = Feelings[GetRandom.Int(0, Feelings.Length)];
            string right = Names[GetRandom.Int(0, Names.Length)];
            var builder = new StringBuilder();

            switch (kind)
            {
                case EcosystemKind.NuGet:
                    return builder.Append(char.ToUpperInvariant(left[0])).Append(left.Skip(1).ToArray()).Append('.').Append(char.ToUpperInvariant(right[0])).Append(right.Skip(1).ToArray()).ToString();
                case EcosystemKind.Docker:
                    return builder.Append(left).Append('/').Append(right).ToString();
                case EcosystemKind.Npm:
                    return builder.Append(GetRandom.Boolean() ? "@" : string.Empty).Append(left).Append('-').Append(right).ToString();
                case EcosystemKind.PyPi:
                    return builder.Append(left).Append('-').Append(right).ToString();
                case EcosystemKind.Maven:
                case EcosystemKind.Gradle:
                    return builder.Append(left).Append('.').Append(right).ToString();
                case EcosystemKind.RubyGem:
                    return builder.Append(left).Append('_').Append(right).ToString();
            }

            return builder.Append(left).Append('_').Append(right).ToString();
        }

        private static readonly string[] Feelings =
        {
            "admiring", "adoring", "affectionate", "agitated", "amazing", "angry", "awesome",
            "blissful", "bold", "boring", "brave", "charming", "clever", "cocky", "cool", "compassionate",
            "competent", "condescending", "confident", "cranky", "crazy",  "dazzling", "determined",
            "distracted", "dreamy", "eager","ecstatic","elastic","elated","elegant","eloquent","epic",
            "fervent","festive","flamboyant","focused","friendly","frosty", "gallant","gifted","goofy",
            "gracious", "happy","hardcore","heuristic","hopeful","hungry", "infallible","inspiring", "jolly",
            "jovial", "keen","kind", "laughing","loving","lucid", "magical","mystifying","modest","musing",
            "naughty","nervous","nifty","nostalgic", "objective","optimistic", "peaceful","pedantic","pensive",
            "practical","priceless", "quirky","quizzical", "recursing","relaxed","reverent","romantic", "sad",
            "serene","sharp","silly","sleepy","stoic","stupefied","suspicious","sweet", "tender","thirsty",
            "trusting", "unruffled","upbeat", "vibrant","vigilant","vigorous", "wizardly","wonderful",
            "xenodochial", "youthful", "zealous","zen",
        };

        private static readonly string[] Names =
        {
            "albattani","allen", "almeida","antonelli", "agnesi","archimedes","ardinghelli","aryabhata", "austin",
            "babbage","banach","banzai","bardeen","bartik","bassi","beaver", "bell", "benz", "bhabha", "bhaskara", "black", "blackburn", "blackwell", "bohr", "booth", "borg", "bose", "boyd", "brahmagupta", "brattain", "brown", "buck", "burnell",
            "cannon", "carson", "cartwright", "cerf", "chandrasekhar", "chaplygin", "chatelet", "chatterjee", "chebyshev", "cohen", "chaum", "clarke", "colden", "cori", "cray", "curran", "curie",
            "darwin", "davinci", "dewdney", "dhawan", "diffie", "dijkstra", "dirac", "driscoll", "dubinsky",
            "easley", "edison", "einstein", "elbakyan", "elgamal", "elion", "ellis", "engelbart", "euclid", "euler",
            "faraday", "feistel", "fermat", "fermi", "feynman", "franklin",
            "gagarin", "galileo", "galois", "ganguly", "gates", "gauss", "germain", "goldberg", "goldstine", "goldwasser", "golick", "goodall", "gould", "greider", "grothendieck",
            "haibt", "hamilton", "haslett", "hawking", "hellman", "heisenberg", "hermann", "herschel", "hertz", "heyrovsky", "hodgkin", "hofstadter", "hoover", "hopper", "hugle", "hypatia",
            "ishizaka", "jackson", "jang", "jennings", "jepsen", "johnson", "joliot", "jones", "kalam", "kapitsa", "kare", "keldysh", "keller",
            "kepler", "khayyam", "khorana", "kilby", "kirch", "knuth", "kowalevski",
            "lalande", "lamarr", "lamport", "leakey", "leavitt", "lederberg", "lehmann", "lewin", "lichterman", "liskov", "lovelace", "lumiere",
            "musk", "mahavira", "margulis", "matsumoto", "maxwell", "mayer", "mccarthy", "mcclintock", "mclaren", "mclean", "mcnulty", "mendel", "mendeleev", "meitner", "meninsky", "merkle", "mestorf", "minsky", "mirzakhani", "moore", "morse", "murdock", "moser",
            "napier", "nash", "neumann", "newton", "nightingale", "nobel", "noether", "northcutt", "noyce",
            "panini", "pare", "pascal", "pasteur", "payne", "perlman", "pike", "poincare", "poitras", "proskuriakova", "ptolemy",
            "raman", "ramanujan", "ride", "montalcini", "ritchie", "rhodes", "robinson", "roentgen", "rosalind", "rubin",
            "saha", "sammet", "sanderson", "shamir", "shannon", "shaw", "shirley", "shockley", "shtern", "sinoussi", "snyder", "solomon", "spence", "stallman", "stonebraker", "sutherland", "swanson", "swartz", "swirles",
            "trump", "taussig", "tereshkova", "tesla", "tharp", "thompson", "torvalds", "tu", "turing",
            "varahamihira", "vaughan", "visvesvaraya", "volhard", "villani",
            "wescoff", "wilbur", "wiles", "williams", "williamson", "wilson", "wing", "wozniak", "wright", "wu",
            "yalow", "yonath", "zhukovsky",
        };

        class DependencyComparer : IEqualityComparer<Dependency>
        {
            public bool Equals(Dependency a, Dependency b) => a.Name.Equals(b.Name) && a.Kind.Equals(b.Kind);
            public int GetHashCode(Dependency a) => a.Name.GetHashCode() ^ a.Kind.GetHashCode();
        }

        class VersionComparer : IComparer<DependencyVersion>
        {
            public int Compare(DependencyVersion x, DependencyVersion y)
            {
                return new Version(x.Version).CompareTo(new Version(y.Version));
            }
        }
    }
}