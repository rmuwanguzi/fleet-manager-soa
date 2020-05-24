using System;
using System.Collections.Generic;
using System.Linq;

namespace MutticoFleet.Service
{
    internal static class DataExtensions
    {
        public static IEnumerable<T> ToFullyLoaded<T>(this IQueryable<T> query)
        {
            return query.ToArray().ToList();
        }
        public static IEnumerable<T> ToFullyLoaded<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToList();
        }
    }
}
