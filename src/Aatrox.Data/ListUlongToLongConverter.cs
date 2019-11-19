using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aatrox.Data
{
    public sealed class ListUlongToLongConverter : ValueConverter<List<ulong>, List<long>>
    {
        private static readonly Expression<Func<List<ulong>, List<long>>> InExpression = x
            => x.Select(x => (long)x).ToList();

        private static readonly Expression<Func<List<long>, List<ulong>>> OutExpression = x
            => x.Select(x => (ulong)x).ToList();

        public ListUlongToLongConverter() : base(InExpression, OutExpression)
        {

        }
    }
}
