using System.Collections.Generic;
using System.Linq;
using Vision.Core;
using Xunit;

namespace Vision.Tests
{
    public class EnumerableExtensionsTests
    {        
        [Theory]
        [InlineData(100, 5,  20)]
        [InlineData(100, 10, 10)]
        [InlineData(100, 25, 4)]
        [InlineData(100, 50, 2)]
        [InlineData(100, 60, 2)]
        [InlineData(100, 90, 2)]
        [InlineData(100, 100, 1)]
        [InlineData(100, 101, 1)]
        [InlineData(100, 200, 1)]
        public void ListChunksAmountToListsize(int size, int chunkBy, int numLists)
        {
            IEnumerable<IEnumerable<int>> result = Enumerable.Range(0, size).ChunkBy(chunkBy);
            Assert.Equal(numLists, result.Count());
        }        
    }
}
