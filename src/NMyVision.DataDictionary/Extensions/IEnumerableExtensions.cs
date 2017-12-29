using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NMyVision.Extensions
{
    [DebuggerStepThrough]
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.List`1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action">The System.Action`1 delegate to perform on each element of the System.Collections.Generic.List`1.</param>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action(item);
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.List`1.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action">The System.Action`1 delegate to perform on each element of the System.Collections.Generic.List`1.</param>
        public static void Each<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            int i = 0;
            foreach (T item in items)
                action(item, i++);
        }
    }

}
