namespace Vision.Web.Core
{
    using Microsoft.AspNetCore.Components.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NavigationService
    {
        public List<NavMenuItem> NavMenuItems { get; }

        public NavigationService()
        {
            NavMenuItems = new List<NavMenuItem>()
            {
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dashboard",               Route = "dashboard", Icon = "fas fa-fw fa-tachometer-alt" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Sources" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version control systems", Route = "sources/vcs", Icon = "fas fa-fw fa-code-branch" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "CI/CD",                   Route = "sources/cicds", Icon = "fas fa-fw fa-cogs" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Registries",              Route = "sources/registries",  Icon = "fas fa-fw fa-archive" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Data" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version control systems", Route = "data/vcs", Icon = "fas fa-fw fa-code-branch" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "CI/CD",                   Route = "data/cicds", Icon = "fas fa-fw fa-cogs" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Registries",              Route = "data/registries",  Icon = "fas fa-fw fa-archive" },                
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies",            Route = "data/dependencies", Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks",              Route = "data/frameworks",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Insights" },
                new NavMenuItem
                {
                    Type = NavMenuItemType.Data,
                    Match = NavLinkMatch.Prefix,
                    Name = "Assets",
                    Route = "insights/assets",
                    Icon = "fas fa-fw fa-table",
                    Children = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>().Select(kind => new NavMenuItem
                    {
                        Type = NavMenuItemType.Data,
                        Match = NavLinkMatch.Prefix,
                        Name = kind.ToString(),
                        Route = $"insights/assets/{(int)kind}",
                        Icon = "fas fa-fw fa-table"
                    })
                    .ToList()
                },
                new NavMenuItem
                {
                    Type = NavMenuItemType.Data,
                    Match = NavLinkMatch.Prefix,
                    Name = "Dependencies",
                    Route = "insights/dependencies",
                    Icon = "fas fa-fw fa-cloud-download-alt",
                    Children = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>().Select(kind => new NavMenuItem
                    {
                        Type = NavMenuItemType.Data,
                        Match = NavLinkMatch.Prefix,
                        Name = kind.ToString(),
                        Route = $"insights/dependencies/{(int)kind}",
                        Icon = "fas fa-fw fa-table"
                    }).ToList()
                },
                new NavMenuItem
                {
                    Type = NavMenuItemType.Data,
                    Match = NavLinkMatch.Prefix,
                    Name = "Repositories",
                    Route = "insights/repositories",
                    Icon = "fas fa-fw fa-cloud-download-alt",
                    Children = Enum.GetValues(typeof(DependencyKind)).Cast<DependencyKind>().Select(kind => new NavMenuItem
                    {
                        Type = NavMenuItemType.Data,
                        Match = NavLinkMatch.Prefix,
                        Name = kind.ToString(),
                        Route = $"insights/repositories/{(int)kind}",
                        Icon = "fas fa-fw fa-table"
                    }).ToList()
                },                
                // new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks", Route = "insights/frameworks", Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Seperator, Name = "Admin" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Tasks",      Route = "admin/tasks", Icon = "fas fa-fw fa-table" }
            };
        }
    }
}


