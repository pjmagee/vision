using System.Collections.Generic;

namespace Vision.Core
{
    public interface IPaginatedList
    {
        int PageIndex { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public interface IPaginatedList<T> : IList<T>, IPaginatedList
    {
     
    }
}
