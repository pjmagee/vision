using Microsoft.AspNetCore.Components.Routing;
using System.Collections.Generic;

namespace Vision.App
{
    public static class NavigationHelper
    {
        public static List<NavMenuItem> NavMenuItems { get; set; }

        static NavigationHelper()
        {
            NavMenuItems = new List<NavMenuItem>()
            {
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dashboard",               Route = "/dashboard", Icon = "fas fa-fw fa-tachometer-alt" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Sources" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version control systems", Route = "/sources/vcs", Icon = "fas fa-fw fa-code-branch" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "CI/CD",                   Route = "/sources/cicds", Icon = "fas fa-fw fa-cogs" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependency registries",   Route = "/sources/registries",  Icon = "fas fa-fw fa-archive" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Data" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version control systems", Route = "/data/vcs", Icon = "fas fa-fw fa-code-branch" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "CI/CD",                   Route = "/data/cicds", Icon = "fas fa-fw fa-cogs" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependency registries",   Route = "/data/registries",  Icon = "fas fa-fw fa-archive" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Assets",                  Route = "/data/assets",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies",            Route = "/data/dependencies", Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks",              Route = "/data/frameworks",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Insights" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Assets",                  Route = "/insights/assets",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies",            Route = "/insights/dependencies", Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks",              Route = "/insights/frameworks",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Admin" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Tasks",                   Route = "/admin/tasks",  Icon = "fas fa-fw fa-table" }
            };
        }
    }
}


