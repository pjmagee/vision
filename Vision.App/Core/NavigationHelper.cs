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
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dashboard", Route = "/", Icon = "fas fa-fw fa-tachometer-alt" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Sources" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version control systems", Route = "/vcs", Icon = "fas fa-fw fa-code-branch" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Build pipelines", Route = "/cicds", Icon = "fas fa-fw fa-cogs" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependency registries", Route = "/registries",  Icon = "fas fa-fw fa-archive" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Data" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Assets", Route = "/assets",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies", Route = "/dependencies",  Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks", Route = "/frameworks",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Admin" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Tasks", Route = "/tasks",  Icon = "fas fa-fw fa-table" }
            };
        }                   
    }
}


