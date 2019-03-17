using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Web.Core
{
    public class PaginatedList<T> : List<T>, IPaginatedList<T>
    {
        public int PageIndex { get; }
        public int TotalPages { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            AddRange(items);
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public static async Task<IPaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int count = await source.CountAsync();
            List<T> items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public IEnumerable<(int, string)> GetPages()
        {
            int current = PageIndex;
            int last = TotalPages;
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
