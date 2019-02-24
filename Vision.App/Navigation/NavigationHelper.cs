using Microsoft.AspNetCore.Components.Routing;
using System.Collections.Generic;
using Vision.App.Shared;

namespace Vision.App
{
    public static class NavigationHelper
    {
        public static List<NavMenuItem> NavMenuItems { get; set; }

        static NavigationHelper()
        {
            NavMenuItems = new List<NavMenuItem>()
            {
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dashboard", Route = "/", Icon = "fab fa-fw fa-tachometer-alt" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Sources" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version Control Servers", Route = "/vcs", Icon = "fab fa-fw fa-git-square" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "CI/CD Servers", Route = "/cicds", Icon = "fab fa-fw fa-git-square" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependency Registries", Route = "/registries",  Icon = "fas fa-fw fa-warehouse" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Data" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Assets", Route = "/assets",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies", Route = "/dependencies",  Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks", Route = "/frameworks",  Icon = "fas fa-fw fa-table" },                
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Tasks", Route = "/tasks",  Icon = "fas fa-fw fa-table" }
            };
        }                   
    }
}


