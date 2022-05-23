using System.Collections.Generic;
using System.Linq;

namespace GraphColoring.Application.Extensions
{
    public static class ListExtensions
    {
        public static List<List<T>> Split<T>(this List<T> source, int chunkSize)
        {
            return source.Select((item, index) => new {index, item})
                .GroupBy(x => x.index % chunkSize)
                .Select(x => x.Select(y => y.item).ToList()).ToList();
        }
    }
}
