namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FizzWare.NBuilder;
    using FizzWare.NBuilder.Generators;
    using Microsoft.AspNetCore.DataProtection;

    public class FakeDataGenerator
    {
        private readonly VisionDbContext context;
        private readonly IDataProtector protector;

        private readonly static IList<DependencyKind> DependencyKinds = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>().ToList();

        public FakeDataGenerator(VisionDbContext context, IDataProtectionProvider provider)
        {
            this.protector = provider.CreateProtector("Registry");
            this.context = context;
        }

        public async Task SeedAsync()
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // VCS Servers
            context.VersionControls.AddRange(GetVersionControls());
            await context.SaveChangesAsync();

            // CI/CD SERVERS
            context.CiCds.AddRange(GetCiCds());
            await context.SaveChangesAsync();

            // REPOSITORIES
            foreach (var vcs in context.VersionControls)
            {
                context.Repositories.AddRange(GetRepositories(vcs));
            }
            await context.SaveChangesAsync();

            // DEPENDENCY REGISTRIES
            context.Registries.AddRange(GetRegistries());
            await context.SaveChangesAsync();

            // FRAMEWORKS
            context.Frameworks.AddRange(GetFrameworks());
            await context.SaveChangesAsync();

            // ASSETS
            foreach (var repository in context.Repositories)
            {
                context.Assets.AddRange(GetAssets(repository));
            }
            await context.SaveChangesAsync();

            // DEPENDENCIES
            List<Repository> repositories = context.Repositories.ToList();
            List<Registry> registries = context.Registries.ToList();

            foreach (DependencyKind kind in DependencyKinds)
            {
                context.Dependencies.AddRange(GetDependencies(registries, repositories, kind));
            }

            await context.SaveChangesAsync();

            // DEPENDENCY VERSIONS
            foreach (Dependency dependency in context.Dependencies)
            {
                context.DependencyVersions.AddRange(GetDependencyVersions(dependency));
            }

            await context.SaveChangesAsync();

            // ASSET DEPENDENCIES
            foreach (Asset asset in context.Assets)
            {
                var assetDependencies = GetAssetDependencies(asset, context.Dependencies.Where(d => d.Kind == asset.Kind).ToList());
                context.AssetDependencies.AddRange(assetDependencies);
            }
            await context.SaveChangesAsync();
            
            // ASSET FRAMEWORKS
            foreach (Asset asset in context.Assets)
            {
                if (asset.Kind != DependencyKind.NuGet) continue;

                IEnumerable<AssetFramework> assetFrameworks = GetAssetFrameworks(asset, context.Frameworks.ToList());
                context.AssetFrameworks.AddRange(assetFrameworks);
            }
            await context.SaveChangesAsync();
        }

        
        private static int GetAssetsByKind(DependencyKind kind)
        {
            switch (kind)
            {
                case DependencyKind.Docker:return GetRandom.Int(1, 5);
                case DependencyKind.NuGet:
                case DependencyKind.Maven: 
                case DependencyKind.Gradle: return GetRandom.Int(1, 10);
                case DependencyKind.Npm: return GetRandom.Int(1, 2);
                case DependencyKind.PyPi:
                case DependencyKind.RubyGem:return GetRandom.Int(1, 5);
            }
            throw new Exception("Unhandled kind for random number of assets");
        }

        private static int GetDependenciesForAsset(DependencyKind kind)
        {
            switch (kind)
            {
                case DependencyKind.Docker: return GetRandom.Int(1, 2);
                case DependencyKind.NuGet:
                case DependencyKind.Maven:
                case DependencyKind.Gradle: return GetRandom.Int(2, 15);
                case DependencyKind.Npm: return GetRandom.Int(2, 15);
                case DependencyKind.RubyGem:
                case DependencyKind.PyPi: return GetRandom.Int(2, 10);
            }
            throw new Exception("Unhandled kind for number of dependencies to return");
        } 


        private IEnumerable<Asset> GetAssets(Repository repository)
        {
            string[] items = repository.WebUrl.Split('/');
            string fileOrFolderName = items[items.Length - 2];
            IList<DependencyKind> kindsOfAssetsInGitRepository = Pick<DependencyKind>.UniqueRandomList(With.Between(1).And(DependencyKinds.Count).Elements).From(DependencyKinds);

            return kindsOfAssetsInGitRepository.SelectMany(kind => Builder<Asset>.CreateListOfSize(GetRandom.Int(1, GetAssetsByKind(kind))).All()
                .With(a => a.Id = Guid.NewGuid())
                .With(a => a.Raw = "FILE CONTENTS")
                .With(a => a.Kind = kind)
                .With((a, fileIndex) => a.Path = GetPathForAsset(fileOrFolderName, fileIndex, kind))
                .With(a => a.Repository = repository)
                .With(a => a.RepositoryId = repository.Id)
                .Build());
        }

        private IEnumerable<Dependency> GetDependencies(IList<Registry> registries, IList<Repository> repositories, DependencyKind kind)
        {
            var privatePicker = new RandomItemPicker<Repository>(repositories, new RandomGenerator());
            var publicPicker = new RandomItemPicker<string>(new[] { "http://github.com", "http://bitbucket.com", "http://gitlab.com" }, new RandomGenerator());

            var names = Enumerable.Range(0, GetRandomDependencies(kind)).Select(x => GetGeneratedNameByKind(kind)).Distinct().ToList();
            var namePicker = new RandomItemPicker<string>(names, new UniqueRandomGenerator());
            RandomItemPicker<Registry> registryPicker = GetRegistryByKind(registries, kind);

            return Builder<Dependency>.CreateListOfSize(names.Count)
                    .All()
                    .With(d => d.Id = Guid.NewGuid())
                    .With(d => d.Name = namePicker.Pick().Trim())
                    .With(d => d.Kind = kind)
                    .With(d => d.Updated = DateTime.Now.Add(new TimeSpan(GetRandom.Int(-150, 0), GetRandom.Int(0, 10), GetRandom.Int(0, 10), GetRandom.Int(0, 10), GetRandom.Int(0, 500))))
                    .Build()
                    .Distinct();
        }

        private IEnumerable<AssetFramework> GetAssetFrameworks(Asset asset, IList<Framework> frameworks)
        {
                   
            var selectedFrameworks = Pick<Framework>.UniqueRandomList(With.Between(1).And(2).Elements).From(frameworks);

            return selectedFrameworks.Select(framework =>
                Builder<AssetFramework>.CreateNew()
                .With(af => af.Id = Guid.NewGuid())
                .With(af => af.Asset = asset)
                .With(af => af.AssetId = asset.Id)
                .With(af => af.Framework = framework)
                .With(af => af.FrameworkId = framework.Id).Build());
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
            var versionCount = GetRandom.Int(1, 10);

            var versions = Builder<DependencyVersion>.CreateListOfSize(versionCount)
                .All()
                .With(dv => dv.IsLatest = false)
                .With(dv => dv.Id = Guid.NewGuid())
                .With(dv => dv.Dependency = dependency)
                .With(dv => dv.DependencyId = dependency.Id)
                .With((dv, index) => 
                {
                    dv.Version = new Version(GetRandom.Int(index, 10), GetRandom.Int(index, 10), GetRandom.Int(index, 10)).ToString();
                })
                .Build().ToList();

            versions.Sort(new VersionComparer());

            versions[versions.Count - 1].IsLatest = true;

            return versions;
        }

        private IEnumerable<VersionControl> GetVersionControls()
        {
            var sources = new VersionControl[]
            {
		         CreateNewVersionControl().With(vc => vc.Endpoint = $"https://bitbucket.xperthr.rbxd.ds:8080/").With(x => x.Kind = VersionControlKind.Bitbucket).Build(),
                 CreateNewVersionControl().With(vc => vc.Endpoint = $"https://bitbucket.accuity.rbxd.ds:8080/").With(x => x.Kind = VersionControlKind.Bitbucket).Build(),
                 CreateNewVersionControl().With(vc => vc.Endpoint = $"https://bitbucket.flightglobal.rbxd.ds:8080/").With(x => x.Kind = VersionControlKind.Gitlab).Build(),
                 CreateNewVersionControl().With(vc => vc.Endpoint = $"https://bitbucket.estatesgazette.rbxd.ds:8080/").With(x => x.Kind = VersionControlKind.Bitbucket).Build(),
                 CreateNewVersionControl().With(vc => vc.Endpoint = $"https://gitlab.icis.rbxd.ds:8080/").With(x => x.Kind = VersionControlKind.Gitlab).Build(),
                 CreateNewVersionControl().With(vc => vc.Endpoint = $"https://gitlab.proagrica.rbxd.ds:8080/").With(x => x.Kind = VersionControlKind.Gitlab).Build(),
                 CreateNewVersionControl().With(vc => vc.Endpoint = $"https://bitbucket.lexisnexis.rbxd.ds:8080/").With(x => x.Kind = VersionControlKind.Bitbucket).Build(),
                 CreateNewVersionControl().With(vc => vc.Endpoint = $"https://gitlab.b2b.regn.net:8080/").With(x => x.Kind = VersionControlKind.Gitlab).Build(),
             };

            return sources;
        }

        private IEnumerable<CiCd> GetCiCds()
        {
            var sources = new CiCd[]
            {
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://jenkins.xperthr.rbxd.ds:8080/").With(x => x.Kind = CiCdKind.Jenkins).Build(),
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://jenkins.accuity.rbxd.ds:8080/").With(x => x.Kind = CiCdKind.Jenkins).Build(),
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://teamcity.flightglobal.rbxd.ds:8080/").With(x => x.Kind = CiCdKind.TeamCity).Build(),
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://teamcity.estatesgazette.rbxd.ds:8080/").With(x => x.Kind = CiCdKind.TeamCity).Build(),
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://teamcity.icis.rbxd.ds:8080/").With(x => x.Kind = CiCdKind.Gitlab).Build(),
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://teamcity.proagrica.rbxd.ds:8080/").With(x => x.Kind = CiCdKind.Gitlab).Build(),
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://teamcity.lexisnexis.rbxd.ds:8080/").With(x => x.Kind = CiCdKind.TeamCity).Build(),
                 CreateNewCiCd().With(ci => ci.Endpoint = $"https://gitlab.b2b.regn.net:8080/").With(x => x.Kind = CiCdKind.Gitlab).Build()
             };

            return sources;
        }

        private ISingleObjectBuilder<VersionControl> CreateNewVersionControl()
        {
            return Builder<VersionControl>.CreateNew()
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.ApiKey = Guid.NewGuid().ToString());
        }

        private ISingleObjectBuilder<CiCd> CreateNewCiCd()
        {
            return Builder<CiCd>.CreateNew()
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.ApiKey = protector.Protect(Guid.NewGuid().ToString()))
                .With(x => x.Username = protector.Protect("Username"))
                .With(x => x.Password = protector.Protect("Password"));
        }

        private ISingleObjectBuilder<Registry> CreateNewRegistry()
        {
            return Builder<Registry>.CreateNew()
                .With(x => x.Id = Guid.NewGuid())
                .With(x => x.ApiKey = protector.Protect(Guid.NewGuid().ToString()))
                .With(x => x.Username = protector.Protect("Username"))
                .With(x => x.Password = protector.Protect("Password"));
        }

        private IEnumerable<Registry> GetRegistries()
        {
            return DependencyKinds.SelectMany(kind => new Registry[]
            {
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.xperthr.rbxd.ds/{kind}/".ToLower())       .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.flight.rbxd.ds/{kind}/".ToLower())        .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.estatesgazette.rbxd.ds/{kind}/".ToLower()).With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.proagrica.rbxd.ds/{kind}/".ToLower())     .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.icis.rbxd.ds/{kind}/".ToLower())          .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.iog.rbxd.ds/{kind}/".ToLower())           .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.lexisnexis.rbxd.ds/{kind}/".ToLower())    .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.flight.rbxd.ds/{kind}/".ToLower())        .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.estatesgazette.rbxd.ds/{kind}/".ToLower()).With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.proagrica.rbxd.ds/{kind}/".ToLower())     .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.icis.rbxd.ds/{kind}/".ToLower())          .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.iog.rbxd.ds/{kind}/".ToLower())           .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nexus.lexisnexis.rbxd.ds/{kind}/".ToLower())    .With(x => x.IsPublic = false).With(x => x.Kind = kind).Build(),                
            })
            .Concat(new[] 
            {
                CreateNewRegistry().With(x => x.Endpoint = $"https://hub.docker.com/")        .With(x => x.IsPublic = true).With(x => x.Kind = DependencyKind.Docker).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://registry.npm.com/")      .With(x => x.IsPublic = true).With(x => x.Kind = DependencyKind.Npm).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://nuget.org.com/v3/")      .With(x => x.IsPublic = true).With(x => x.Kind = DependencyKind.NuGet).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://registry.maven.com/")    .With(x => x.IsPublic = true).With(x => x.Kind = DependencyKind.Maven).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://registry.maven.com/")    .With(x => x.IsPublic = true).With(x => x.Kind = DependencyKind.Gradle).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://registry.python.com/")   .With(x => x.IsPublic = true).With(x => x.Kind = DependencyKind.PyPi).Build(),
                CreateNewRegistry().With(x => x.Endpoint = $"https://registry.rubygems.com/") .With(x => x.IsPublic = true).With(x => x.Kind = DependencyKind.RubyGem).Build()
            });
        }
        
        private IEnumerable<Framework> GetFrameworks()
        {
            var netStandards = new[] { "netstandard1.0","netstandard1.1", "netstandard1.2", "netstandard1.3", "netstandard1.4", "netstandard1.5", "netstandard1.6", "netstandard2.0" };
            var netCores = new[]  { "netcoreapp1.0", "netcoreapp1.1", "netcoreapp2.0", "netcoreapp2.1", "netcoreapp2.2" };
            var netFrameworks = new[] {  "net11", "net20", "net35", "net40", "net403", "net45", "net451", "net452", "net46", "net461", "net462", "net47", "net471", "net472" };

            return netStandards.Concat(netCores).Concat(netFrameworks)
                .Select(framework => Builder<Framework>.CreateNew()
                    .With(f => f.Id = Guid.NewGuid())
                    .With(f => f.Version = framework).Build());
        }

        private IEnumerable<Repository> GetRepositories(VersionControl source)
        {
            var repoNames = Enumerable.Range(0, GetRandom.Int(30, 800)).Select(x => GetGeneratedNameByKind(DependencyKind.Npm).Trim('@')).Distinct().ToList();
            var repoPicker = new RandomItemPicker<string>(repoNames, new UniqueRandomGenerator());

            return Builder<Repository>.CreateListOfSize(GetRandom.Int(1, repoNames.Count))
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
                .With(r => r.VersionControl = source)
                .With(r => r.VersionControlId= source.Id)
                .Build();
        }

        private static RandomItemPicker<Registry> GetRegistryByKind(IEnumerable<Registry> sources, DependencyKind kind)
        {
            return new RandomItemPicker<Registry>(sources.Where(x => x.Kind == kind).ToList(), new RandomGenerator());
        }

        private static int GetRandomDependencies(DependencyKind kind)
        {
            switch (kind)
            {
                case DependencyKind.Docker: return GetRandom.Int(10, 30);
                case DependencyKind.Maven:
                case DependencyKind.Gradle:
                case DependencyKind.NuGet: return GetRandom.Int(50, 300);
                case DependencyKind.Npm: return GetRandom.Int(50, 300);
                case DependencyKind.PyPi:
                case DependencyKind.RubyGem: return GetRandom.Int(50, 300);
            }

            throw new Exception("Cannot pick random item picker for dependency sources for chosen dependency kind");
        }

        private static string GetPathForAsset(string fileOrFolderName, int fileIndex, DependencyKind kind)
        {
            string ext = kind.GetFileExtension();

            bool isFirstFile = fileIndex == 0;

            switch (kind)
            {
                case DependencyKind.Docker:  return isFirstFile ? $"docker/src/{ext}" : $"docker/src/{fileIndex}.{ext}";
                case DependencyKind.RubyGem: return isFirstFile ? $"ruby/src/{fileOrFolderName}/{ext}" : $"ruby/src/{fileOrFolderName}/{fileIndex}/{ext}";
                case DependencyKind.Maven: return isFirstFile ? $"java/src/{fileOrFolderName}/{fileOrFolderName}.{ext}" : $"java/src/{fileOrFolderName}/{fileOrFolderName}.{fileIndex}/{fileOrFolderName}.{fileIndex}.{ext}";
                case DependencyKind.Gradle: return isFirstFile ? $"java/src/{fileOrFolderName}/{ext}" : $"java/src/{fileOrFolderName}/{fileOrFolderName}.{fileIndex}/{ext}";
                case DependencyKind.NuGet: return isFirstFile ? $"dotnet/src/{fileOrFolderName}/{fileOrFolderName}.{ext}" : $"dotnet/src/{fileOrFolderName}/{fileOrFolderName}.{fileIndex}/{fileOrFolderName}.{fileIndex}.{ext}";
                case DependencyKind.Npm: return isFirstFile ? $"nodejs/src/{fileOrFolderName}/{ext}" : $"nodejs/src/{fileOrFolderName}/{fileIndex}/{ext}";
                case DependencyKind.PyPi: return isFirstFile ? $"python/src/{fileOrFolderName}/{ext}" : $"python/src/{fileOrFolderName}/{fileIndex}/{ext}";
            }

            throw new Exception("Unhandled kind");
        }

        private static string GetGeneratedNameByKind(DependencyKind kind)
        {
            string left = Feelings[GetRandom.Int(0, Feelings.Length)];
            string right = Names[GetRandom.Int(0, Names.Length)];
            var builder = new StringBuilder();

            switch (kind)
            {
                case DependencyKind.NuGet:
                    return builder.Append(char.ToUpperInvariant(left[0])).Append(left.Skip(1).ToArray()).Append('.').Append(char.ToUpperInvariant(right[0])).Append(right.Skip(1).ToArray()).ToString();
                case DependencyKind.Docker:
                    return builder.Append(left).Append('/').Append(right).ToString();
                case DependencyKind.Npm:
                    return builder.Append(GetRandom.Boolean() ? "@" : string.Empty).Append(left).Append('-').Append(right).ToString();
                case DependencyKind.PyPi:
                    return new StringBuilder().Append(left).Append('-').Append(right).ToString();
                case DependencyKind.Maven:
                case DependencyKind.Gradle:
                    return builder.Append(left).Append('.').Append(right).ToString();
                case DependencyKind.RubyGem:
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
            "albattani",
            "allen",
            "almeida",
            "antonelli",
            "agnesi",
            "archimedes",
            "ardinghelli",
            "aryabhata",
            "austin",
            "babbage",
            "banach",
            "banzai",
            "bardeen",
            "bartik",
            "bassi",
            "beaver",
            "bell",
            "benz",
            "bhabha",
            "bhaskara",
            "black",
            "blackburn",
            "blackwell",
            "bohr",
            "booth",
            "borg",
            "bose",
            "boyd",
            "brahmagupta",
            "brattain",
            "brown",
            "buck",
            "burnell",
            "cannon",
            "carson",
            "cartwright",
            "cerf",
            "chandrasekhar",
            "chaplygin",
		    // Émilie du Châtelet - French natural philosopher, mathematician, physicist, and author during the early 1730s, known for her translation of and commentary on Isaac Newton's book Principia containing basic laws of physics. https://en.wikipedia.org/wiki/%C3%89milie_du_Ch%C3%A2telet
		    "chatelet",
		    // Asima Chatterjee was an Indian organic chemist noted for her research on vinca alkaloids, development of drugs for treatment of epilepsy and malaria - https://en.wikipedia.org/wiki/Asima_Chatterjee
		    "chatterjee",
		    // Pafnuty Chebyshev - Russian mathematician. He is known fo his works on probability, statistics, mechanics, analytical geometry and number theory https://en.wikipedia.org/wiki/Pafnuty_Chebyshev
		    "chebyshev",
		    // Bram Cohen - American computer programmer and author of the BitTorrent peer-to-peer protocol. https://en.wikipedia.org/wiki/Bram_Cohen
		    "cohen",
		    // David Lee Chaum - American computer scientist and cryptographer. Known for his seminal contributions in the field of anonymous communication. https://en.wikipedia.org/wiki/David_Chaum
		    "chaum",
		    // Joan Clarke - Bletchley Park code breaker during the Second World War who pioneered techniques that remained top secret for decades. Also an accomplished numismatist https://en.wikipedia.org/wiki/Joan_Clarke
		    "clarke",
		    // Jane Colden - American botanist widely considered the first female American botanist - https://en.wikipedia.org/wiki/Jane_Colden
		    "colden",
		    // Gerty Theresa Cori - American biochemist who became the third woman—and first American woman—to win a Nobel Prize in science, and the first woman to be awarded the Nobel Prize in Physiology or Medicine. Cori was born in Prague. https://en.wikipedia.org/wiki/Gerty_Cori
		    "cori",
		    // Seymour Roger Cray was an American electrical engineer and supercomputer architect who designed a series of computers that were the fastest in the world for decades. https://en.wikipedia.org/wiki/Seymour_Cray
		    "cray",
		    // This entry reflects a husband and wife team who worked together:
		    // Joan Curran was a Welsh scientist who developed radar and invented chaff, a radar countermeasure. https://en.wikipedia.org/wiki/Joan_Curran
		    // Samuel Curran was an Irish physicist who worked alongside his wife during WWII and invented the proximity fuse. https://en.wikipedia.org/wiki/Samuel_Curran
		    "curran",
		    // Marie Curie discovered radioactivity. https://en.wikipedia.org/wiki/Marie_Curie.
		    "curie",
		    // Charles Darwin established the principles of natural evolution. https://en.wikipedia.org/wiki/Charles_Darwin.
		    "darwin",
		    // Leonardo Da Vinci invented too many things to list here. https://en.wikipedia.org/wiki/Leonardo_da_Vinci.
		    "davinci",
		    // A. K. (Alexander Keewatin) Dewdney, Canadian mathematician, computer scientist, author and filmmaker. Contributor to Scientific American's "Computer Recreations" from 1984 to 1991. Author of Core War (program), The Planiverse, The Armchair Universe, The Magic Machine, The New Turing Omnibus, and more. https://en.wikipedia.org/wiki/Alexander_Dewdney
		    "dewdney",
		    // Satish Dhawan - Indian mathematician and aerospace engineer, known for leading the successful and indigenous development of the Indian space programme. https://en.wikipedia.org/wiki/Satish_Dhawan
		    "dhawan",
		    // Bailey Whitfield Diffie - American cryptographer and one of the pioneers of public-key cryptography. https://en.wikipedia.org/wiki/Whitfield_Diffie
		    "diffie",
		    // Edsger Wybe Dijkstra was a Dutch computer scientist and mathematical scientist. https://en.wikipedia.org/wiki/Edsger_W._Dijkstra.
		    "dijkstra",
		    // Paul Adrien Maurice Dirac - English theoretical physicist who made fundamental contributions to the early development of both quantum mechanics and quantum electrodynamics. https://en.wikipedia.org/wiki/Paul_Dirac
		    "dirac",
		    // Agnes Meyer Driscoll - American cryptanalyst during World Wars I and II who successfully cryptanalysed a number of Japanese ciphers. She was also the co-developer of one of the cipher machines of the US Navy, the CM. https://en.wikipedia.org/wiki/Agnes_Meyer_Driscoll
		    "driscoll",
		    // Donna Dubinsky - played an integral role in the development of personal digital assistants (PDAs) serving as CEO of Palm, Inc. and co-founding Handspring. https://en.wikipedia.org/wiki/Donna_Dubinsky
		    "dubinsky",
		    // Annie Easley - She was a leading member of the team which developed software for the Centaur rocket stage and one of the first African-Americans in her field. https://en.wikipedia.org/wiki/Annie_Easley
		    "easley",
		    // Thomas Alva Edison, prolific inventor https://en.wikipedia.org/wiki/Thomas_Edison
		    "edison",
		    // Albert Einstein invented the general theory of relativity. https://en.wikipedia.org/wiki/Albert_Einstein
		    "einstein",
		    // Alexandra Asanovna Elbakyan (Russian: Алекса́ндра Аса́новна Элбакя́н) is a Kazakhstani graduate student, computer programmer, internet pirate in hiding, and the creator of the site Sci-Hub. Nature has listed her in 2016 in the top ten people that mattered in science, and Ars Technica has compared her to Aaron Swartz. - https://en.wikipedia.org/wiki/Alexandra_Elbakyan
		    "elbakyan",
		    // Taher A. ElGamal - Egyptian cryptographer best known for the ElGamal discrete log cryptosystem and the ElGamal digital signature scheme. https://en.wikipedia.org/wiki/Taher_Elgamal
		    "elgamal",
		    // Gertrude Elion - American biochemist, pharmacologist and the 1988 recipient of the Nobel Prize in Medicine - https://en.wikipedia.org/wiki/Gertrude_Elion
		    "elion",
		    // SpaceX, Boring Company, Tesla, PayPal, OpenAI, and many others
		    "musk",
		    // Donald Trump - Bless his heart
		    "trump",
		    // James Henry Ellis - British engineer and cryptographer employed by the GCHQ. Best known for conceiving for the first time, the idea of public-key cryptography. https://en.wikipedia.org/wiki/James_H._Ellis
		    "ellis",
		    // Douglas Engelbart gave the mother of all demos: https://en.wikipedia.org/wiki/Douglas_Engelbart
		    "engelbart",
		    // Euclid invented geometry. https://en.wikipedia.org/wiki/Euclid
		    "euclid",
		    // Leonhard Euler invented large parts of modern mathematics. https://de.wikipedia.org/wiki/Leonhard_Euler
		    "euler",
		    // Michael Faraday - British scientist who contributed to the study of electromagnetism and electrochemistry. https://en.wikipedia.org/wiki/Michael_Faraday
		    "faraday",
		    // Horst Feistel - German-born American cryptographer who was one of the earliest non-government researchers to study the design and theory of block ciphers. Co-developer of DES and Lucifer. Feistel networks, a symmetric structure used in the construction of block ciphers are named after him. https://en.wikipedia.org/wiki/Horst_Feistel
		    "feistel",
		    // Pierre de Fermat pioneered several aspects of modern mathematics. https://en.wikipedia.org/wiki/Pierre_de_Fermat
		    "fermat",
		    // Enrico Fermi invented the first nuclear reactor. https://en.wikipedia.org/wiki/Enrico_Fermi.
		    "fermi",
		    // Richard Feynman was a key contributor to quantum mechanics and particle physics. https://en.wikipedia.org/wiki/Richard_Feynman
		    "feynman",
		    // Benjamin Franklin is famous for his experiments in electricity and the invention of the lightning rod.
		    "franklin",
		    // Yuri Alekseyevich Gagarin - Soviet pilot and cosmonaut, best known as the first human to journey into outer space. https://en.wikipedia.org/wiki/Yuri_Gagarin
		    "gagarin",
		    // Galileo was a founding father of modern astronomy, and faced politics and obscurantism to establish scientific truth.  https://en.wikipedia.org/wiki/Galileo_Galilei
		    "galileo",
		    // Évariste Galois - French mathematician whose work laid the foundations of Galois theory and group theory, two major branches of abstract algebra, and the subfield of Galois connections, all while still in his late teens. https://en.wikipedia.org/wiki/%C3%89variste_Galois
		    "galois",
		    // Kadambini Ganguly - Indian physician, known for being the first South Asian female physician, trained in western medicine, to graduate in South Asia. https://en.wikipedia.org/wiki/Kadambini_Ganguly
		    "ganguly",
		    // William Henry "Bill" Gates III is an American business magnate, philanthropist, investor, computer programmer, and inventor. https://en.wikipedia.org/wiki/Bill_Gates
		    "gates",
		    // Johann Carl Friedrich Gauss - German mathematician who made significant contributions to many fields, including number theory, algebra, statistics, analysis, differential geometry, geodesy, geophysics, mechanics, electrostatics, magnetic fields, astronomy, matrix theory, and optics. https://en.wikipedia.org/wiki/Carl_Friedrich_Gauss
		    "gauss",
		    // Marie-Sophie Germain - French mathematician, physicist and philosopher. Known for her work on elasticity theory, number theory and philosophy. https://en.wikipedia.org/wiki/Sophie_Germain
		    "germain",
		    // Adele Goldberg, was one of the designers and developers of the Smalltalk language. https://en.wikipedia.org/wiki/Adele_Goldberg_(computer_scientist)
		    "goldberg",
		    // Adele Goldstine, born Adele Katz, wrote the complete technical description for the first electronic digital computer, ENIAC. https://en.wikipedia.org/wiki/Adele_Goldstine
		    "goldstine",
		    // Shafi Goldwasser is a computer scientist known for creating theoretical foundations of modern cryptography. Winner of 2012 ACM Turing Award. https://en.wikipedia.org/wiki/Shafi_Goldwasser
		    "goldwasser",
		    // James Golick, all around gangster.
		    "golick",
		    // Jane Goodall - British primatologist, ethologist, and anthropologist who is considered to be the world's foremost expert on chimpanzees - https://en.wikipedia.org/wiki/Jane_Goodall
		    "goodall",
		    // Stephen Jay Gould was was an American paleontologist, evolutionary biologist, and historian of science. He is most famous for the theory of punctuated equilibrium - https://en.wikipedia.org/wiki/Stephen_Jay_Gould
		    "gould",
		    // Carolyn Widney Greider - American molecular biologist and joint winner of the 2009 Nobel Prize for Physiology or Medicine for the discovery of telomerase. https://en.wikipedia.org/wiki/Carol_W._Greider
		    "greider",
		    // Alexander Grothendieck - German-born French mathematician who became a leading figure in the creation of modern algebraic geometry. https://en.wikipedia.org/wiki/Alexander_Grothendieck
		    "grothendieck",
		    // Lois Haibt - American computer scientist, part of the team at IBM that developed FORTRAN - https://en.wikipedia.org/wiki/Lois_Haibt
		    "haibt",
		    // Margaret Hamilton - Director of the Software Engineering Division of the MIT Instrumentation Laboratory, which developed on-board flight software for the Apollo space program. https://en.wikipedia.org/wiki/Margaret_Hamilton_(scientist)
		    "hamilton",
		    // Caroline Harriet Haslett - English electrical engineer, electricity industry administrator and champion of women's rights. Co-author of British Standard 1363 that specifies AC power plugs and sockets used across the United Kingdom (which is widely considered as one of the safest designs). https://en.wikipedia.org/wiki/Caroline_Haslett
		    "haslett",
		    // Stephen Hawking pioneered the field of cosmology by combining general relativity and quantum mechanics. https://en.wikipedia.org/wiki/Stephen_Hawking
		    "hawking",
		    // Martin Edward Hellman - American cryptologist, best known for his invention of public-key cryptography in co-operation with Whitfield Diffie and Ralph Merkle. https://en.wikipedia.org/wiki/Martin_Hellman
		    "hellman",
		    // Werner Heisenberg was a founding father of quantum mechanics. https://en.wikipedia.org/wiki/Werner_Heisenberg
		    "heisenberg",
		    // Grete Hermann was a German philosopher noted for her philosophical work on the foundations of quantum mechanics. https://en.wikipedia.org/wiki/Grete_Hermann
		    "hermann",
		    // Caroline Lucretia Herschel - German astronomer and discoverer of several comets. https://en.wikipedia.org/wiki/Caroline_Herschel
		    "herschel",
		    // Heinrich Rudolf Hertz - German physicist who first conclusively proved the existence of the electromagnetic waves. https://en.wikipedia.org/wiki/Heinrich_Hertz
		    "hertz",
		    // Jaroslav Heyrovský was the inventor of the polarographic method, father of the electroanalytical method, and recipient of the Nobel Prize in 1959. His main field of work was polarography. https://en.wikipedia.org/wiki/Jaroslav_Heyrovsk%C3%BD
		    "heyrovsky",
		    // Dorothy Hodgkin was a British biochemist, credited with the development of protein crystallography. She was awarded the Nobel Prize in Chemistry in 1964. https://en.wikipedia.org/wiki/Dorothy_Hodgkin
		    "hodgkin",
		    // Douglas R. Hofstadter is an American professor of cognitive science and author of the Pulitzer Prize and American Book Award-winning work Goedel, Escher, Bach: An Eternal Golden Braid in 1979. A mind-bending work which coined Hofstadter's Law: "It always takes longer than you expect, even when you take into account Hofstadter's Law." https://en.wikipedia.org/wiki/Douglas_Hofstadter
		    "hofstadter",
		    // Erna Schneider Hoover revolutionized modern communication by inventing a computerized telephone switching method. https://en.wikipedia.org/wiki/Erna_Schneider_Hoover
		    "hoover",
		    // Grace Hopper developed the first compiler for a computer programming language and  is credited with popularizing the term "debugging" for fixing computer glitches. https://en.wikipedia.org/wiki/Grace_Hopper
		    "hopper",
		    // Frances Hugle, she was an American scientist, engineer, and inventor who contributed to the understanding of semiconductors, integrated circuitry, and the unique electrical principles of microscopic materials. https://en.wikipedia.org/wiki/Frances_Hugle
		    "hugle",
		    // Hypatia - Greek Alexandrine Neoplatonist philosopher in Egypt who was one of the earliest mothers of mathematics - https://en.wikipedia.org/wiki/Hypatia
		    "hypatia",
		    // Teruko Ishizaka - Japanese scientist and immunologist who co-discovered the antibody class Immunoglobulin E. https://en.wikipedia.org/wiki/Teruko_Ishizaka
		    "ishizaka",
		    // Mary Jackson, American mathematician and aerospace engineer who earned the highest title within NASA's engineering department - https://en.wikipedia.org/wiki/Mary_Jackson_(engineer)
		    "jackson",
		    // Yeong-Sil Jang was a Korean scientist and astronomer during the Joseon Dynasty; he invented the first metal printing press and water gauge. https://en.wikipedia.org/wiki/Jang_Yeong-sil
		    "jang",
		    // Betty Jennings - one of the original programmers of the ENIAC. https://en.wikipedia.org/wiki/ENIAC - https://en.wikipedia.org/wiki/Jean_Bartik
		    "jennings",
		    // Mary Lou Jepsen, was the founder and chief technology officer of One Laptop Per Child (OLPC), and the founder of Pixel Qi. https://en.wikipedia.org/wiki/Mary_Lou_Jepsen
		    "jepsen",
		    // Katherine Coleman Goble Johnson - American physicist and mathematician contributed to the NASA. https://en.wikipedia.org/wiki/Katherine_Johnson
		    "johnson",
		    // Irène Joliot-Curie - French scientist who was awarded the Nobel Prize for Chemistry in 1935. Daughter of Marie and Pierre Curie. https://en.wikipedia.org/wiki/Ir%C3%A8ne_Joliot-Curie
		    "joliot",
		    // Karen Spärck Jones came up with the concept of inverse document frequency, which is used in most search engines today. https://en.wikipedia.org/wiki/Karen_Sp%C3%A4rck_Jones
		    "jones",
		    // A. P. J. Abdul Kalam - is an Indian scientist aka Missile Man of India for his work on the development of ballistic missile and launch vehicle technology - https://en.wikipedia.org/wiki/A._P._J._Abdul_Kalam
		    "kalam",
		    // Sergey Petrovich Kapitsa (Russian: Серге́й Петро́вич Капи́ца; 14 February 1928 – 14 August 2012) was a Russian physicist and demographer. He was best known as host of the popular and long-running Russian scientific TV show, Evident, but Incredible. His father was the Nobel laureate Soviet-era physicist Pyotr Kapitsa, and his brother was the geographer and Antarctic explorer Andrey Kapitsa. - https://en.wikipedia.org/wiki/Sergey_Kapitsa
		    "kapitsa",
		    // Susan Kare, created the icons and many of the interface elements for the original Apple Macintosh in the 1980s, and was an original employee of NeXT, working as the Creative Director. https://en.wikipedia.org/wiki/Susan_Kare
		    "kare",
		    // Mstislav Keldysh - a Soviet scientist in the field of mathematics and mechanics, academician of the USSR Academy of Sciences (1946), President of the USSR Academy of Sciences (1961–1975), three times Hero of Socialist Labor (1956, 1961, 1971), fellow of the Royal Society of Edinburgh (1968). https://en.wikipedia.org/wiki/Mstislav_Keldysh
		    "keldysh",
		    // Mary Kenneth Keller, Sister Mary Kenneth Keller became the first American woman to earn a PhD in Computer Science in 1965. https://en.wikipedia.org/wiki/Mary_Kenneth_Keller
		    "keller",
		    // Johannes Kepler, German astronomer known for his three laws of planetary motion - https://en.wikipedia.org/wiki/Johannes_Kepler
		    "kepler",
		    // Omar Khayyam - Persian mathematician, astronomer and poet. Known for his work on the classification and solution of cubic equations, for his contribution to the understanding of Euclid's fifth postulate and for computing the length of a year very accurately. https://en.wikipedia.org/wiki/Omar_Khayyam
		    "khayyam",
		    // Har Gobind Khorana - Indian-American biochemist who shared the 1968 Nobel Prize for Physiology - https://en.wikipedia.org/wiki/Har_Gobind_Khorana
		    "khorana",
		    // Jack Kilby invented silicone integrated circuits and gave Silicon Valley its name. - https://en.wikipedia.org/wiki/Jack_Kilby
		    "kilby",
		    // Maria Kirch - German astronomer and first woman to discover a comet - https://en.wikipedia.org/wiki/Maria_Margarethe_Kirch
		    "kirch",
		    // Donald Knuth - American computer scientist, author of "The Art of Computer Programming" and creator of the TeX typesetting system. https://en.wikipedia.org/wiki/Donald_Knuth
		    "knuth",
		    // Sophie Kowalevski - Russian mathematician responsible for important original contributions to analysis, differential equations and mechanics - https://en.wikipedia.org/wiki/Sofia_Kovalevskaya
		    "kowalevski",
		    // Marie-Jeanne de Lalande - French astronomer, mathematician and cataloguer of stars - https://en.wikipedia.org/wiki/Marie-Jeanne_de_Lalande
		    "lalande",
		    // Hedy Lamarr - Actress and inventor. The principles of her work are now incorporated into modern Wi-Fi, CDMA and Bluetooth technology. https://en.wikipedia.org/wiki/Hedy_Lamarr
		    "lamarr",
		    // Leslie B. Lamport - American computer scientist. Lamport is best known for his seminal work in distributed systems and was the winner of the 2013 Turing Award. https://en.wikipedia.org/wiki/Leslie_Lamport
		    "lamport",
		    // Mary Leakey - British paleoanthropologist who discovered the first fossilized Proconsul skull - https://en.wikipedia.org/wiki/Mary_Leakey
		    "leakey",
		    // Henrietta Swan Leavitt - she was an American astronomer who discovered the relation between the luminosity and the period of Cepheid variable stars. https://en.wikipedia.org/wiki/Henrietta_Swan_Leavitt
		    "leavitt",
		    // Esther Miriam Zimmer Lederberg - American microbiologist and a pioneer of bacterial genetics. https://en.wikipedia.org/wiki/Esther_Lederberg
		    "lederberg",
		    // Inge Lehmann - Danish seismologist and geophysicist. Known for discovering in 1936 that the Earth has a solid inner core inside a molten outer core. https://en.wikipedia.org/wiki/Inge_Lehmann
		    "lehmann",
		    // Daniel Lewin - Mathematician, Akamai co-founder, soldier, 9/11 victim-- Developed optimization techniques for routing traffic on the internet. Died attempting to stop the 9-11 hijackers. https://en.wikipedia.org/wiki/Daniel_Lewin
		    "lewin",
		    // Ruth Lichterman - one of the original programmers of the ENIAC. https://en.wikipedia.org/wiki/ENIAC - https://en.wikipedia.org/wiki/Ruth_Teitelbaum
		    "lichterman",
		    // Barbara Liskov - co-developed the Liskov substitution principle. Liskov was also the winner of the Turing Prize in 2008. - https://en.wikipedia.org/wiki/Barbara_Liskov
		    "liskov",
		    // Ada Lovelace invented the first algorithm. https://en.wikipedia.org/wiki/Ada_Lovelace (thanks James Turnbull)
		    "lovelace",
		    // Auguste and Louis Lumière - the first filmmakers in history - https://en.wikipedia.org/wiki/Auguste_and_Louis_Lumi%C3%A8re
		    "lumiere",
		    // Mahavira - Ancient Indian mathematician during 9th century AD who discovered basic algebraic identities - https://en.wikipedia.org/wiki/Mah%C4%81v%C4%ABra_(mathematician)
		    "mahavira",
		    // Lynn Margulis (b. Lynn Petra Alexander) - an American evolutionary theorist and biologist, science author, educator, and popularizer, and was the primary modern proponent for the significance of symbiosis in evolution. - https://en.wikipedia.org/wiki/Lynn_Margulis
		    "margulis",
		    // Yukihiro Matsumoto - Japanese computer scientist and software programmer best known as the chief designer of the RubyGem programming language. https://en.wikipedia.org/wiki/Yukihiro_Matsumoto
		    "matsumoto",
		    // James Clerk Maxwell - Scottish physicist, best known for his formulation of electromagnetic theory. https://en.wikipedia.org/wiki/James_Clerk_Maxwell
		    "maxwell",
		    // Maria Mayer - American theoretical physicist and Nobel laureate in Physics for proposing the nuclear shell model of the atomic nucleus - https://en.wikipedia.org/wiki/Maria_Mayer
		    "mayer",
		    // John McCarthy invented LISP: https://en.wikipedia.org/wiki/John_McCarthy_(computer_scientist)
		    "mccarthy",
		    // Barbara McClintock - a distinguished American cytogeneticist, 1983 Nobel Laureate in Physiology or Medicine for discovering transposons. https://en.wikipedia.org/wiki/Barbara_McClintock
		    "mcclintock",
		    // Anne Laura Dorinthea McLaren - British developmental biologist whose work helped lead to human in-vitro fertilisation. https://en.wikipedia.org/wiki/Anne_McLaren
		    "mclaren",
		    // Malcolm McLean invented the modern shipping container: https://en.wikipedia.org/wiki/Malcom_McLean
		    "mclean",
		    // Kay McNulty - one of the original programmers of the ENIAC. https://en.wikipedia.org/wiki/ENIAC - https://en.wikipedia.org/wiki/Kathleen_Antonelli
		    "mcnulty",
		    // Gregor Johann Mendel - Czech scientist and founder of genetics. https://en.wikipedia.org/wiki/Gregor_Mendel
		    "mendel",
		    // Dmitri Mendeleev - a chemist and inventor. He formulated the Periodic Law, created a farsighted version of the periodic table of elements, and used it to correct the properties of some already discovered elements and also to predict the properties of eight elements yet to be discovered. https://en.wikipedia.org/wiki/Dmitri_Mendeleev
		    "mendeleev",
		    // Lise Meitner - Austrian/Swedish physicist who was involved in the discovery of nuclear fission. The element meitnerium is named after her - https://en.wikipedia.org/wiki/Lise_Meitner
		    "meitner",
		    // Carla Meninsky, was the game designer and programmer for Atari 2600 games Dodge 'Em and Warlords. https://en.wikipedia.org/wiki/Carla_Meninsky
		    "meninsky",
		    // Ralph C. Merkle - American computer scientist, known for devising Merkle's puzzles - one of the very first schemes for public-key cryptography. Also, inventor of Merkle trees and co-inventor of the Merkle-Damgård construction for building collision-resistant cryptographic hash functions and the Merkle-Hellman knapsack cryptosystem. https://en.wikipedia.org/wiki/Ralph_Merkle
		    "merkle",
		    // Johanna Mestorf - German prehistoric archaeologist and first female museum director in Germany - https://en.wikipedia.org/wiki/Johanna_Mestorf
		    "mestorf",
		    // Marvin Minsky - Pioneer in Artificial Intelligence, co-founder of the MIT's AI Lab, won the Turing Award in 1969. https://en.wikipedia.org/wiki/Marvin_Minsky
		    "minsky",
		    // Maryam Mirzakhani - an Iranian mathematician and the first woman to win the Fields Medal. https://en.wikipedia.org/wiki/Maryam_Mirzakhani
		    "mirzakhani",
		    // Gordon Earle Moore - American engineer, Silicon Valley founding father, author of Moore's law. https://en.wikipedia.org/wiki/Gordon_Moore
		    "moore",
		    // Samuel Morse - contributed to the invention of a single-wire telegraph system based on European telegraphs and was a co-developer of the Morse code - https://en.wikipedia.org/wiki/Samuel_Morse
		    "morse",
		    // Ian Murdock  founder of the Debian project - https://en.wikipedia.org/wiki/Ian_Murdock
		    "murdock",
		    // May-Britt Moser - Nobel prize winner neuroscientist who contributed to the discovery of grid cells in the brain. https://en.wikipedia.org/wiki/May-Britt_Moser
		    "moser",
		    // John Napier of Merchiston - Scottish landowner known as an astronomer, mathematician and physicist. Best known for his discovery of logarithms. https://en.wikipedia.org/wiki/John_Napier
		    "napier",
		    // John Forbes Nash, Jr. - American mathematician who made fundamental contributions to game theory, differential geometry, and the study of partial differential equations. https://en.wikipedia.org/wiki/John_Forbes_Nash_Jr.
		    "nash",
		    // John von Neumann - todays computer architectures are based on the von Neumann architecture. https://en.wikipedia.org/wiki/Von_Neumann_architecture
		    "neumann",
		    // Isaac Newton invented classic mechanics and modern optics. https://en.wikipedia.org/wiki/Isaac_Newton
		    "newton",
		    // Florence Nightigale, more prominently known as a nurse, was also the first female member of the Royal Statistical Society and a pioneer in statistical graphics https://en.wikipedia.org/wiki/Florence_Nightingale#Statistics_and_sanitary_reform
		    "nightingale",
		    // Alfred Nobel - a Swedish chemist, engineer, innovator, and armaments manufacturer (inventor of dynamite) - https://en.wikipedia.org/wiki/Alfred_Nobel
		    "nobel",
		    // Emmy Noether, erman mathematician. Noether's Theorem is named after her. https://en.wikipedia.org/wiki/Emmy_Noether
		    "noether",
		    // Poppy Northcutt. Poppy Northcutt was the first woman to work as part of NASA’s Mission Control. http://www.businessinsider.com/poppy-northcutt-helped-apollo-astronauts-2014-12?op=1
		    "northcutt",
		    // Robert Noyce invented silicone integrated circuits and gave Silicon Valley its name. - https://en.wikipedia.org/wiki/Robert_Noyce
		    "noyce",
		    // Panini - Ancient Indian linguist and grammarian from 4th century CE who worked on the world's first formal system - https://en.wikipedia.org/wiki/P%C4%81%E1%B9%87ini#Comparison_with_modern_formal_systems
		    "panini",
		    // Ambroise Pare invented modern surgery. https://en.wikipedia.org/wiki/Ambroise_Par%C3%A9
		    "pare",
		    // Blaise Pascal, French mathematician, physicist, and inventor - https://en.wikipedia.org/wiki/Blaise_Pascal
		    "pascal",
		    // Louis Pasteur discovered vaccination, fermentation and pasteurization. https://en.wikipedia.org/wiki/Louis_Pasteur.
		    "pasteur",
		    // Cecilia Payne-Gaposchkin was an astronomer and astrophysicist who, in 1925, proposed in her Ph.D. thesis an explanation for the composition of stars in terms of the relative abundances of hydrogen and helium. https://en.wikipedia.org/wiki/Cecilia_Payne-Gaposchkin
		    "payne",
		    // Radia Perlman is a software designer and network engineer and most famous for her invention of the spanning-tree protocol (STP). https://en.wikipedia.org/wiki/Radia_Perlman
		    "perlman",
		    // Rob Pike was a key contributor to Unix, Plan 9, the X graphic system, utf-8, and the Go programming language. https://en.wikipedia.org/wiki/Rob_Pike
		    "pike",
		    // Henri Poincaré made fundamental contributions in several fields of mathematics. https://en.wikipedia.org/wiki/Henri_Poincar%C3%A9
		    "poincare",
		    // Laura Poitras is a director and producer whose work, made possible by open source crypto tools, advances the causes of truth and freedom of information by reporting disclosures by whistleblowers such as Edward Snowden. https://en.wikipedia.org/wiki/Laura_Poitras
		    "poitras",
		    // Tat’yana Avenirovna Proskuriakova (Russian: Татья́на Авени́ровна Проскуряко́ва) (January 23 [O.S. January 10] 1909 – August 30, 1985) was a Russian-American Mayanist scholar and archaeologist who contributed significantly to the deciphering of Maya hieroglyphs, the writing system of the pre-Columbian Maya civilization of Mesoamerica. https://en.wikipedia.org/wiki/Tatiana_Proskouriakoff
		    "proskuriakova",
		    // Claudius Ptolemy - a Greco-Egyptian writer of Alexandria, known as a mathematician, astronomer, geographer, astrologer, and poet of a single epigram in the Greek Anthology - https://en.wikipedia.org/wiki/Ptolemy
		    "ptolemy",
		    // C. V. Raman - Indian physicist who won the Nobel Prize in 1930 for proposing the Raman effect. - https://en.wikipedia.org/wiki/C._V._Raman
		    "raman",
		    // Srinivasa Ramanujan - Indian mathematician and autodidact who made extraordinary contributions to mathematical analysis, number theory, infinite series, and continued fractions. - https://en.wikipedia.org/wiki/Srinivasa_Ramanujan
		    "ramanujan",
		    // Sally Kristen Ride was an American physicist and astronaut. She was the first American woman in space, and the youngest American astronaut. https://en.wikipedia.org/wiki/Sally_Ride
		    "ride",
		    // Rita Levi-Montalcini - Won Nobel Prize in Physiology or Medicine jointly with colleague Stanley Cohen for the discovery of nerve growth factor (https://en.wikipedia.org/wiki/Rita_Levi-Montalcini)
		    "montalcini",
		    // Dennis Ritchie - co-creator of UNIX and the C programming language. - https://en.wikipedia.org/wiki/Dennis_Ritchie
		    "ritchie",
		    // Ida Rhodes - American pioneer in computer programming, designed the first computer used for Social Security. https://en.wikipedia.org/wiki/Ida_Rhodes
		    "rhodes",
		    // Julia Hall Bowman Robinson - American mathematician renowned for her contributions to the fields of computability theory and computational complexity theory. https://en.wikipedia.org/wiki/Julia_Robinson
		    "robinson",
		    // Wilhelm Conrad Röntgen - German physicist who was awarded the first Nobel Prize in Physics in 1901 for the discovery of X-rays (Röntgen rays). https://en.wikipedia.org/wiki/Wilhelm_R%C3%B6ntgen
		    "roentgen",
		    // Rosalind Franklin - British biophysicist and X-ray crystallographer whose research was critical to the understanding of DNA - https://en.wikipedia.org/wiki/Rosalind_Franklin
		    "rosalind",
		    // Vera Rubin - American astronomer who pioneered work on galaxy rotation rates. https://en.wikipedia.org/wiki/Vera_Rubin
		    "rubin",
		    // Meghnad Saha - Indian astrophysicist best known for his development of the Saha equation, used to describe chemical and physical conditions in stars - https://en.wikipedia.org/wiki/Meghnad_Saha
		    "saha",
		    // Jean E. Sammet developed FORMAC, the first widely used computer language for symbolic manipulation of mathematical formulas. https://en.wikipedia.org/wiki/Jean_E._Sammet
		    "sammet",
		    // Mildred Sanderson - American mathematician best known for Sanderson's theorem concerning modular invariants. https://en.wikipedia.org/wiki/Mildred_Sanderson
		    "sanderson",
		    // Adi Shamir - Israeli cryptographer whose numerous inventions and contributions to cryptography include the Ferge Fiat Shamir identification scheme, the Rivest Shamir Adleman (RSA) public-key cryptosystem, the Shamir's secret sharing scheme, the breaking of the Merkle-Hellman cryptosystem, the TWINKLE and TWIRL factoring devices and the discovery of differential cryptanalysis (with Eli Biham). https://en.wikipedia.org/wiki/Adi_Shamir
		    "shamir",
		    // Claude Shannon - The father of information theory and founder of digital circuit design theory. (https://en.wikipedia.org/wiki/Claude_Shannon)
		    "shannon",
		    // Carol Shaw - Originally an Atari employee, Carol Shaw is said to be the first female video game designer. https://en.wikipedia.org/wiki/Carol_Shaw_(video_game_designer)
		    "shaw",
		    // Dame Stephanie "Steve" Shirley - Founded a software company in 1962 employing women working from home. https://en.wikipedia.org/wiki/Steve_Shirley
		    "shirley",
		    // William Shockley co-invented the transistor - https://en.wikipedia.org/wiki/William_Shockley
		    "shockley",
		    // Lina Solomonovna Stern (or Shtern; Russian: Лина Соломоновна Штерн; 26 August 1878 – 7 March 1968) was a Soviet biochemist, physiologist and humanist whose medical discoveries saved thousands of lives at the fronts of World War II. She is best known for her pioneering work on blood–brain barrier, which she described as hemato-encephalic barrier in 1921. https://en.wikipedia.org/wiki/Lina_Stern
		    "shtern",
		    // Françoise Barré-Sinoussi - French virologist and Nobel Prize Laureate in Physiology or Medicine; her work was fundamental in identifying HIV as the cause of AIDS. https://en.wikipedia.org/wiki/Fran%C3%A7oise_Barr%C3%A9-Sinoussi
		    "sinoussi",
		    // Betty Snyder - one of the original programmers of the ENIAC. https://en.wikipedia.org/wiki/ENIAC - https://en.wikipedia.org/wiki/Betty_Holberton
		    "snyder",
		    // Cynthia Solomon - Pioneer in the fields of artificial intelligence, computer science and educational computing. Known for creation of Logo, an educational programming language.  https://en.wikipedia.org/wiki/Cynthia_Solomon
		    "solomon",
		    // Frances Spence - one of the original programmers of the ENIAC. https://en.wikipedia.org/wiki/ENIAC - https://en.wikipedia.org/wiki/Frances_Spence
		    "spence",
		    // Richard Matthew Stallman - the founder of the Free Software movement, the GNU project, the Free Software Foundation, and the League for Programming Freedom. He also invented the concept of copyleft to protect the ideals of this movement, and enshrined this concept in the widely-used GPL (General Public License) for software. https://en.wikiquote.org/wiki/Richard_Stallman
		    "stallman",
		    // Michael Stonebraker is a database research pioneer and architect of Ingres, Postgres, VoltDB and SciDB. Winner of 2014 ACM Turing Award. https://en.wikipedia.org/wiki/Michael_Stonebraker
		    "stonebraker",
		    // Ivan Edward Sutherland - American computer scientist and Internet pioneer, widely regarded as the father of computer graphics. https://en.wikipedia.org/wiki/Ivan_Sutherland
		    "sutherland",
		    // Janese Swanson (with others) developed the first of the Carmen Sandiego games. She went on to found Girl Tech. https://en.wikipedia.org/wiki/Janese_Swanson
		    "swanson",
		    // Aaron Swartz was influential in creating RSS, Markdown, Creative Commons, Reddit, and much of the internet as we know it today. He was devoted to freedom of information on the web. https://en.wikiquote.org/wiki/Aaron_Swartz
		    "swartz",
		    // Bertha Swirles was a theoretical physicist who made a number of contributions to early quantum theory. https://en.wikipedia.org/wiki/Bertha_Swirles
		    "swirles",
		    // Helen Brooke Taussig - American cardiologist and founder of the field of paediatric cardiology. https://en.wikipedia.org/wiki/Helen_B._Taussig
		    "taussig",
		    // Valentina Tereshkova is a Russian engineer, cosmonaut and politician. She was the first woman to fly to space in 1963. In 2013, at the age of 76, she offered to go on a one-way mission to Mars. https://en.wikipedia.org/wiki/Valentina_Tereshkova
		    "tereshkova",
		    // Nikola Tesla invented the AC electric system and every gadget ever used by a James Bond villain. https://en.wikipedia.org/wiki/Nikola_Tesla
		    "tesla",
		    // Marie Tharp - American geologist and oceanic cartographer who co-created the first scientific map of the Atlantic Ocean floor. Her work led to the acceptance of the theories of plate tectonics and continental drift. https://en.wikipedia.org/wiki/Marie_Tharp
		    "tharp",
		    // Ken Thompson - co-creator of UNIX and the C programming language - https://en.wikipedia.org/wiki/Ken_Thompson
		    "thompson",
		    // Linus Torvalds invented Linux and Git. https://en.wikipedia.org/wiki/Linus_Torvalds
		    "torvalds",
		    // Youyou Tu - Chinese pharmaceutical chemist and educator known for discovering artemisinin and dihydroartemisinin, used to treat malaria, which has saved millions of lives. Joint winner of the 2015 Nobel Prize in Physiology or Medicine. https://en.wikipedia.org/wiki/Tu_Youyou
		    "tu",
		    // Alan Turing was a founding father of computer science. https://en.wikipedia.org/wiki/Alan_Turing.
		    "turing",
		    // Varahamihira - Ancient Indian mathematician who discovered trigonometric formulae during 505-587 CE - https://en.wikipedia.org/wiki/Var%C4%81hamihira#Contributions
		    "varahamihira",
		    // Dorothy Vaughan was a NASA mathematician and computer programmer on the SCOUT launch vehicle program that put America's first satellites into space - https://en.wikipedia.org/wiki/Dorothy_Vaughan
		    "vaughan",
		    // Sir Mokshagundam Visvesvaraya - is a notable Indian engineer.  He is a recipient of the Indian Republic's highest honour, the Bharat Ratna, in 1955. On his birthday, 15 September is celebrated as Engineer's Day in India in his memory - https://en.wikipedia.org/wiki/Visvesvaraya
		    "visvesvaraya",
		    // Christiane Nüsslein-Volhard - German biologist, won Nobel Prize in Physiology or Medicine in 1995 for research on the genetic control of embryonic development. https://en.wikipedia.org/wiki/Christiane_N%C3%BCsslein-Volhard
		    "volhard",
		    // Cédric Villani - French mathematician, won Fields Medal, Fermat Prize and Poincaré Price for his work in differential geometry and statistical mechanics. https://en.wikipedia.org/wiki/C%C3%A9dric_Villani
		    "villani",
		    // Marlyn Wescoff - one of the original programmers of the ENIAC. https://en.wikipedia.org/wiki/ENIAC - https://en.wikipedia.org/wiki/Marlyn_Meltzer
		    "wescoff",
		    // Sylvia B. Wilbur - British computer scientist who helped develop the ARPANET, was one of the first to exchange email in the UK and a leading researcher in computer-supported collaborative work. https://en.wikipedia.org/wiki/Sylvia_Wilbur
		    "wilbur",
		    // Andrew Wiles - Notable British mathematician who proved the enigmatic Fermat's Last Theorem - https://en.wikipedia.org/wiki/Andrew_Wiles
		    "wiles",
		    // Roberta Williams, did pioneering work in graphical adventure games for personal computers, particularly the King's Quest series. https://en.wikipedia.org/wiki/Roberta_Williams
		    "williams",
		    // Malcolm John Williamson - British mathematician and cryptographer employed by the GCHQ. Developed in 1974 what is now known as Diffie-Hellman key exchange (Diffie and Hellman first published the scheme in 1976). https://en.wikipedia.org/wiki/Malcolm_J._Williamson
		    "williamson",
		    // Sophie Wilson designed the first Acorn Micro-Computer and the instruction set for ARM processors. https://en.wikipedia.org/wiki/Sophie_Wilson
		    "wilson",
		    // Jeannette Wing - co-developed the Liskov substitution principle. - https://en.wikipedia.org/wiki/Jeannette_Wing
		    "wing",
		    // Steve Wozniak invented the Apple I and Apple II. https://en.wikipedia.org/wiki/Steve_Wozniak
		    "wozniak",
		    // The Wright brothers, Orville and Wilbur - credited with inventing and building the world's first successful airplane and making the first controlled, powered and sustained heavier-than-air human flight - https://en.wikipedia.org/wiki/Wright_brothers
		    "wright",
		    // Chien-Shiung Wu - Chinese-American experimental physicist who made significant contributions to nuclear physics. https://en.wikipedia.org/wiki/Chien-Shiung_Wu
		    "wu",
		    // Rosalyn Sussman Yalow - Rosalyn Sussman Yalow was an American medical physicist, and a co-winner of the 1977 Nobel Prize in Physiology or Medicine for development of the radioimmunoassay technique. https://en.wikipedia.org/wiki/Rosalyn_Sussman_Yalow
		    "yalow",
		    // Ada Yonath - an Israeli crystallographer, the first woman from the Middle East to win a Nobel prize in the sciences. https://en.wikipedia.org/wiki/Ada_Yonath
		    "yonath",
		    // Nikolay Yegorovich Zhukovsky (Russian: Никола́й Его́рович Жуко́вский, January 17 1847 – March 17, 1921) was a Russian scientist, mathematician and engineer, and a founding father of modern aero- and hydrodynamics. Whereas contemporary scientists scoffed at the idea of human flight, Zhukovsky was the first to undertake the study of airflow. He is often called the Father of Russian Aviation. https://en.wikipedia.org/wiki/Nikolay_Yegorovich_Zhukovsky
		    "zhukovsky",
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