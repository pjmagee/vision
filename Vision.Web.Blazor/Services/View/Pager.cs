using System.Collections.Generic;
using Vision.Core;

namespace Vision.Web.Blazor.Services
{
    public class Pager : IPager
    {
        public IEnumerable<(int, string)> GetPages(IPaginatedList list)
        {
            int current = list.PageIndex;
            int last = list.TotalPages;
            int delta = 4;
            int left = current - delta;
            int right = current + delta + 1;

            List<int> range = new List<int>();
            List<(int, string)> rangeWithDots = new List<(int, string)>();

            int l = 0;

            for (int i = 1; i <= last; i++)
            {
                if (i == 1 || i == last || i >= left && i < right)
                {
                    range.Add(i);
                }
            }

            foreach (var item in range)
            {
                if (l > 0)
                {
                    if (item - l == 2)
                    {
                        var result = (l + 1);

                        rangeWithDots.Add((result, (l + 1).ToString()));
                    }
                    else if (item - l != 1)
                    {
                        rangeWithDots.Add((item, "..."));
                    }
                }
                rangeWithDots.Add((item, item.ToString()));
                l = item;
            }

            return rangeWithDots;
        }
    }
}