using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using System.Data.Entity;

namespace PITCSurveySvc.Models
{
    public static class ModelConverterHelpers
    {

        public static IEnumerable<T> WhereEx<T>(this DbSet<T> set, Expression<Func<T, bool>> predicate) where T : class
        {
            var dbResult = set.Where(predicate);

            var offlineResult = set.Local.Where(predicate.Compile());

            return offlineResult.Union(dbResult);
        }

    }
}