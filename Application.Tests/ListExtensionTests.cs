using System.Collections.Generic;
using System.Linq;
using GraphColoring.Application.Extensions;
using Xunit;

namespace Application.Tests
{
    public class ListExtensionTests
    {
        [Theory]
        [InlineData(new int[]{1,1,1,1,1}, 1)]
        [InlineData(new int[]{1,1,1,1,1,1,1,1,1,1,1}, 2)]
        [InlineData(new int[]{1,1,1,1,1,1,1,1,1,1,1}, 7)]
        public void ListExtensionShouldSplitListInto(int[] array, int chunks)
        {
            var list = array.ToList();
            var splits = list.Split(chunks);
            Assert.Equal(chunks, splits.Count);
        }
    }
}