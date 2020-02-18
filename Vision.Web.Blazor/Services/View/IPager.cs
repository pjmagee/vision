using System.Collections.Generic;
using Vision.Core;

namespace Vision.Web.Blazor.Services
{
    public interface IPager
    {
        IEnumerable<(int, string)> GetPages(IPaginatedList list);
    }
}