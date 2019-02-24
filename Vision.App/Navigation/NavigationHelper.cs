using Microsoft.AspNetCore.Components.Routing;
using System.Collections.Generic;
using Vision.App.Shared;

namespace Vision.App
{
    public static class NavigationHelper
    {
        public static List<NavMenuItem> NavMenuItems { get; set; }
        public static Dictionary<string, BreadCrumbItem[]> BreadCrumbItems { get; set; }

        static NavigationHelper()
        {
            NavMenuItems = new List<NavMenuItem>()
            {
                new NavMenuItem { Type = NavMenuItemType.Datasource, Match = NavLinkMatch.Prefix, Name = "Dashboard", Route = "/", Icon = "fab fa-fw fa-tachometer-alt" },
                new NavMenuItem { Type = NavMenuItemType.Datasource, Match = NavLinkMatch.Prefix, Name = "Version Control Servers", Route = "/vcs", Icon = "fab fa-fw fa-git-square" },
                new NavMenuItem { Type = NavMenuItemType.Datasource, Match = NavLinkMatch.Prefix, Name = "CI/CD Servers", Route = "/cicds", Icon = "fab fa-fw fa-git-square" },
                new NavMenuItem { Type = NavMenuItemType.Datasource, Match = NavLinkMatch.Prefix, Name = "Dependency Registries", Route = "/registries",  Icon = "fas fa-fw fa-warehouse" },

                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Assets", Route = "/assets",  Icon = "fas fa-fw fa-table" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Dependencies", Route = "/dependencies",  Icon = "fas fa-fw fa-cloud-download-alt" },
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Frameworks", Route = "/frameworks",  Icon = "fas fa-fw fa-table" },                
                new NavMenuItem { Type = NavMenuItemType.Data, Match = NavLinkMatch.Prefix, Name = "Tasks", Route = "/tasks",  Icon = "fas fa-fw fa-table" }
            };

            BreadCrumbItems = new Dictionary<string, BreadCrumbItem[]>()
            {
                { "vcs", new BreadCrumbItem [] { new BreadCrumbItem { Name = "Version Control Servers", Active = true, Route = "/vcs"  } } },
                { "cicds", new BreadCrumbItem [] { new BreadCrumbItem { Name = "CI/CD Servers", Active = true, Route = "/cicds"  } } },
                { "registries", new BreadCrumbItem [] { new BreadCrumbItem { Name = "Dependency Registries", Active = true, Route = "/registries"  } } },
                { "dependencies", new BreadCrumbItem [] { new BreadCrumbItem { Name = "Dependency Registries", Active = true, Route = "/registries"  } } },
                { "dependency", new BreadCrumbItem [] { new BreadCrumbItem { Name = "Dependency Registries", Active = true, Route = "/registries"  } } },
                { "assets", new BreadCrumbItem [] { new BreadCrumbItem { Name = "Dependency Registries", Active = true, Route = "/registries"  } } },
                { "asset", new BreadCrumbItem [] { new BreadCrumbItem { Name = "Dependency Registries", Active = true, Route = "/asset"  } } },
            };
        }                   
    }


}


