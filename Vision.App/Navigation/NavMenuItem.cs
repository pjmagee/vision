﻿using Microsoft.AspNetCore.Components.Routing;
using System;

namespace Vision.App
{
    public class NavMenuItem
    {
        public string Name { get; set; }
        public string Route { get; set; }
        public NavLinkMatch Match { get; set; }
        public string Icon { get; set; }
        public NavMenuItemType Type { get; set; }
        public bool IsActive { get; set; }
    }

    public enum NavMenuItemType
    {
        Seperator,
        Data
    }
}