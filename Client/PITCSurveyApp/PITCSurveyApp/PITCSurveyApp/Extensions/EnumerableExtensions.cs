using System;
using System.Collections.Generic;

namespace PITCSurveyApp.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{T}"/>.
    /// </summary>
    static class EnumerableExtensions
    {
        /// <summary>
        /// Gets the minimum item from the collection based on the selected comparable property.
        /// </summary>
        /// <typeparam name="T">Element type of collection.</typeparam>
        /// <typeparam name="TComparable">Type of comparable property.</typeparam>
        /// <param name="enumerable">The collection.</param>
        /// <param name="selector">The comparable property selector.</param>
        /// <returns>The minimum item in the collection or a default value if the collection is empty.</returns>
        public static T MinByOrDefault<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> selector)
            where TComparable : IComparable
        {
            return MinByOrDefault(enumerable, selector, ComparableComparer<TComparable>.Instance);
        }

        /// <summary>
        /// Gets the minimum item from the collection based on the selected comparable property.
        /// </summary>
        /// <typeparam name="T">Element type of collection.</typeparam>
        /// <typeparam name="TComparable">Type of comparable property.</typeparam>
        /// <param name="enumerable">The collection.</param>
        /// <param name="selector">The comparable property selector.</param>
        /// <param name="comparer">The comparer for the selected comparable property.</param>
        /// <returns>The minimum item in the collection or a default value if the collection is empty.</returns>
        public static T MinByOrDefault<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> selector, IComparer<TComparable> comparer)
        {
            return MinOrMaxByOrDefault(enumerable, selector, (x, y) => comparer.Compare(x, y) < 0);
        }

        /// <summary>
        /// Gets the maximum item from the collection based on the selected comparable property.
        /// </summary>
        /// <typeparam name="T">Element type of collection.</typeparam>
        /// <typeparam name="TComparable">Type of comparable property.</typeparam>
        /// <param name="enumerable">The collection.</param>
        /// <param name="selector">The comparable property selector.</param>
        /// <returns>The maximum item in the collection or a default value if the collection is empty.</returns>
        public static T MaxByOrDefault<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> selector)
            where TComparable : IComparable
        {
            return MaxByOrDefault(enumerable, selector, ComparableComparer<TComparable>.Instance);
        }

        /// <summary>
        /// Gets the maximum item from the collection based on the selected comparable property.
        /// </summary>
        /// <typeparam name="T">Element type of collection.</typeparam>
        /// <typeparam name="TComparable">Type of comparable property.</typeparam>
        /// <param name="enumerable">The collection.</param>
        /// <param name="selector">The comparable property selector.</param>
        /// <param name="comparer">The comparer for the selected comparable property.</param>
        /// <returns>The maximum item in the collection or a default value if the collection is empty.</returns>
        public static T MaxByOrDefault<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> selector, IComparer<TComparable> comparer)
        {
            return MinOrMaxByOrDefault(enumerable, selector, (x, y) => comparer.Compare(x, y) > 0);
        }

        private static T MinOrMaxByOrDefault<T, TComparable>(
            this IEnumerable<T> enumerable,
            Func<T, TComparable> selector, 
            Func<TComparable, TComparable, bool> comparator)
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
                var best = selector(value);
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    var comparand = selector(current);
                    if (comparator(comparand, best))
                    {
                        value = current;
                        best = comparand;
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
