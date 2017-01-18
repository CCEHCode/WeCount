using System;
using System.Collections.Generic;

namespace PITCSurveyApp.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{T}"/>.
    /// </summary>
    static class EnumerableExtensions
    {
        public static T MaxByOrDefault<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> selector)
            where TComparable : IComparable
        {
            return MaxByOrDefault(enumerable, selector, ComparableComparer<TComparable>.Instance);
        }

        public static T MaxByOrDefault<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> selector, IComparer<TComparable> comparer)
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
                    if (comparer.Compare(comparand, max) > 0)
                    {
                        value = current;
                        max = comparand;
                    }
                }

                return value;
            }
        }

        class ComparableComparer<T> : IComparer<T>
            where T : IComparable
        {
            private ComparableComparer() { }

            public static readonly ComparableComparer<T> Instance = new ComparableComparer<T>();

            public int Compare(T x, T y)
            {
                return x.CompareTo(y);
            }
        }
    }
}
