using GraphColoring.Application.Extensions;
using Xunit;

namespace Application.Tests
{
    public class SolutionExtensionTests
    {
        [Theory]
        [InlineData(new int[]{1,2,3,4}, new int[]{2,3,4,5})]
        [InlineData(new int[]{2,3,4,5}, new int[]{1,2,3,4})]
        [InlineData(new int[]{1,1,1,1}, new int[]{2,2,2,2})]
        [InlineData(new int[]{2,2,2,2}, new int[]{1,1,1,1})]
        [InlineData(new int[]{1,2,2,1}, new int[]{2,1,1,2})]
        [InlineData(new int[]{2,1,1,2}, new int[]{1,2,2,1})]
        [InlineData(new int[]{2,1,1,3,2}, new int[]{1,2,2,3,1})]
        [InlineData(new int[]{3,1,1,2,3}, new int[]{1,2,2,3,1})]
        public void SolutionExtensionShouldMatch(int[] first, int[] second)
        {
            var result = first.Matching(second);
            Assert.True(result);
        }
        
        [Theory]
        [InlineData(new int[]{1,2,3,4}, new int[]{2,7,7,5})]
        [InlineData(new int[]{2,7,7,5}, new int[]{1,2,3,4})]
        [InlineData(new int[]{1,1,1,1}, new int[]{2,1,2,2})]
        [InlineData(new int[]{2,1,2,2}, new int[]{1,1,1,1})]
        [InlineData(new int[]{1,2,2,1}, new int[]{2,2,1,2})]
        [InlineData(new int[]{2,2,1,2}, new int[]{1,2,2,1})]
        [InlineData(new int[]{1,2,2,3,1}, new int[]{2,2,2,3,2})]
        [InlineData(new int[]{2,2,2,3,2}, new int[]{1,2,2,3,1})]
        [InlineData(new int[]{1,2,3,3,1}, new int[]{3,1,1,2,3})]
        [InlineData(new int[]{1,2,3,3}, new int[]{3,1,1,2,3})]
        public void SolutionExtensionShouldNotMatch(int[] first, int[] second)
        {
            var result = first.Matching(second);
            Assert.False(result);
        }
    }
}