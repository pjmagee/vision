using Microsoft.AspNetCore.Components.Routing;
using System.Collections.Generic;
using Vision.App.Shared;

namespace Vision.App
{
    public static class NavigationHelper
    {
        public static IEnumerable<NavMenuItem> NavMenuItems { get; set; }
        public static IEnumerable<NavMenuItem> BreadCrumbItems { get; set; }

        static NavigationHelper()
        {
            NavMenuItems = new[]
            {
                new NavMenuItem { Type = NavMenuItemType.Datasource, Match = NavLinkMatch.Prefix, Name = "Git", Route = "/sources", Icon = "fab fa-fw fa-git-square" },
                new NavMenuItem { Type = NavMenuItemType.Datasource, Match = NavLinkMatch.Prefix, Name = "Registries", Route = "/registries",  Icon = "fas fa-fw fa-warehouse" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Assets", Route = "/assets",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies", Route = "/dependencies",  Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks", Route = "/frameworks",  Icon = "fas fa-fw fa-table" }
            };
        }                   
    }
}
