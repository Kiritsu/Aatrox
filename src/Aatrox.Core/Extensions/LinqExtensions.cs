using System;
using System.Collections.Generic;
using System.Linq;

namespace Aatrox.Core.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> func)
        {
            return items.GroupBy(func).Select(x => x.First());
        }
    }
}
