using System.Collections.Generic;

namespace Vision.Web.Core
{
    public class Vcs : Entity
    {
        public VersionControlKind Kind { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public bool IsEnabled { get; set; }
        public virtual ICollection<VcsRepository> Repositories { get; set; }

        public static IEnumerable<Vcs> MockData()
        {
            yield return new Vcs
            {
                ApiKey = "MjM0OTkxODc4NzgzOh4dBCIn5N5DxaMqpreiybxov3e2",
                IsEnabled = true,
                Kind = VersionControlKind.Bitbucket,
                Endpoint = "http://stash.xpa.rbxd.ds:8090"
            };
        }
    }
}
