using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Routing;

namespace Vision.Web.Core
{
    public class NavMenuItem
    {
        public string Name { get; set; }
        public string Route { get; set; }
        public NavLinkMatch Match { get; set; }
        public string Icon { get; set; }
        public NavMenuItemType Type { get; set; }
        public bool IsActive { get; set; }
        public List<NavMenuItem> Children { get; set; } = new List<NavMenuItem>();
    }
}
