using System.Collections.Generic;

namespace Vision.Web.Core
{
    public interface IPaginatedList
    {
        int PageIndex { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }

        IEnumerable<(int, string)> GetPages();
    }

    public interface IPaginatedList<T> : IList<T>, IPaginatedList
    {

    }
}
