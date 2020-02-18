using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;

namespace Vision.Web.Blazor.Services
{

    public class NavigationService
    {
        public List<NavMenuItem> NavMenuItems { get; }

        public NavigationService()
        {
            NavMenuItems = new List<NavMenuItem>()
            {
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dashboard",       Route = "dashboard", Icon = "fas fa-fw fa-tachometer-alt" },
                new NavMenuItem { Type = NavMenuItemType.Seperator,                         Name = "Sources" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version control", Route = "sources/vcs", Icon = "fas fa-fw fa-code-branch" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "CI/CD",           Route = "sources/cicds", Icon = "fas fa-fw fa-cogs" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Registries",      Route = "sources/registries",  Icon = "fas fa-fw fa-archive" },
                new NavMenuItem { Type = NavMenuItemType.Seperator,                         Name = "Data" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Version control", Route = "data/vcs", Icon = "fas fa-fw fa-code-branch" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Assets",          Route = "data/assets", Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "CI/CD",           Route = "data/cicds", Icon = "fas fa-fw fa-cogs" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Registries",      Route = "data/registries",  Icon = "fas fa-fw fa-archive" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies",    Route = "data/dependencies", Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Ecosystems",      Route = "data/ecosystems",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Vulnerabilities", Route = "data/vulnerabilities",  Icon = "fas fa-fw fa-shield-alt" },
                new NavMenuItem { Type = NavMenuItemType.Seperator,                         Name = "Admin" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Tasks",           Route = "admin/tasks",        Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Repositories",    Route = "admin/repositories", Icon = "fas fa-fw fa-code-branch" }
            };
        }
    }
}