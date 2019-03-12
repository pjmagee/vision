namespace Vision.Web.Core
{
    using System;

    public static class AssetExtensions
    {

        public static DependencyKind GetDependencyKind(this Asset asset) => asset.Path.GetDependencyKind();

        public static bool Is(this Asset asset, DependencyKind kind) => kind == asset.GetDependencyKind();

        public static bool IsNot(this Asset asset, DependencyKind kind) => !asset.Is(kind);

        public static bool IsSupported(this Asset asset) => asset.Path.IsSupported();        
    }
}

