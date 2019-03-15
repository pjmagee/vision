﻿namespace Vision.Web.Core
{
    using System.Threading.Tasks;

    public interface IVersionService
    {
        bool Supports(DependencyKind kind);
        Task<DependencyVersion> GetLatestMetaDataAsync(Dependency dependency);
    }
}
