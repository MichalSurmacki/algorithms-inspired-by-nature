using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace GraphColoring.Application.Extensions
{
    public static class ListExtensions
    {
        public static List<List<T>> Split<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
