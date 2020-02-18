namespace Vision.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => (Index: i, Value: x))
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }

        public static int[] ToIntArray<T>(this IEnumerable<T> kinds) where T : Enum => (kinds ?? Enumerable.Empty<T>()).Cast<int>().ToArray();
    }
}
