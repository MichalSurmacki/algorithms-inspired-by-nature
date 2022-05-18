using System.Linq;

namespace GraphColoring.Application.Extensions
{
    public static class SolutionsExtensions
    {
        public static bool Matching<T>(this T[] first, T[] second)
        {
            foreach (var valueFirst in first)
            {
                var valueFirstIndexes = first
                    .Select((v ,i) => new {v, i})
                    .Where(x => x.v.Equals(valueFirst))
                    .Select(x => x.i)
                    .ToArray();

                var valueSecond = second[valueFirstIndexes[0]];
                for (var i = 0; i < second.Length; i++)
                {
                    if ((valueFirstIndexes.Contains(i) && !second[i].Equals(valueSecond)) || (!valueFirstIndexes.Contains(i) && second[i].Equals(valueSecond)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}