using System;
using System.Collections.Generic;

namespace PITCSurveyApp.Extensions
{
    static class EnumerableExtensions
    {
        public static T MaxByOrDefault<T, TComparand>(this IEnumerable<T> enumerable, Func<T, TComparand> selector)
            where TComparand : IComparable
        {
            if (enumerable == null)
            {
                throw new NullReferenceException(nameof(enumerable));
            }

            if (selector == null)
            {
                throw new NullReferenceException(nameof(selector));
            }

            using (var enumerator = enumerable.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return default(T);
                }

                var value = enumerator.Current;
                var max = selector(value);
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    var comparand = selector(current);
                    if (comparand.CompareTo(max) > 0)
                    {
                        value = current;
                        max = comparand;
                    }
                }

                return value;
            }
        }
    }
}
