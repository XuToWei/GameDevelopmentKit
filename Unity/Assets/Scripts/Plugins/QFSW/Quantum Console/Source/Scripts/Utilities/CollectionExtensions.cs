using System;
using System.Collections.Generic;
using System.Linq;

namespace QFSW.QC.Utilities
{
    public static class CollectionExtensions
    {
        /// <summary>Inverts the key/value relationship between the items in the dictionary.</summary>
        /// <returns>Dictionary with the inverted relationship.</returns>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            Dictionary<TValue, TKey> dictionary = new Dictionary<TValue, TKey>();
            foreach (KeyValuePair<TKey, TValue> item in source)
            {
                if (!dictionary.ContainsKey(item.Value))
                {
                    dictionary.Add(item.Value, item.Key);
                }
            }

            return dictionary;
        }

        /// <summary>Gets a sub array of an existing array.</summary>
        /// <param name="index">Index to take the sub array from.</param>
        /// <param name="length">The length of the sub array.</param>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>Skips the last element in the sequence.</summary>
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            using (IEnumerator<T> enumurator = source.GetEnumerator())
            {
                if (enumurator.MoveNext())
                {
                    for (T value = enumurator.Current; enumurator.MoveNext(); value = enumurator.Current)
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>Reverses the order of the sequence.</summary>
        public static IEnumerable<T> Reversed<T>(this IReadOnlyList<T> source)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                yield return source[i];
            }
        }

        /// <summary>
        /// Creates a distinct stream based on a custom predicate.
        /// </summary>
        /// <typeparam name="TValue">The type of the IEnumerable.</typeparam>
        /// <typeparam name="TDistinct">The type of the value to test for distinctness.</typeparam>
        /// <param name="source">The source IEnumerable.</param>
        /// <param name="predicate">The custom distinct item producer.</param>
        /// <returns>The distinct stream.</returns>
        public static IEnumerable<TValue> DistinctBy<TValue, TDistinct>(this IEnumerable<TValue> source, Func<TValue, TDistinct> predicate)
        {
            HashSet<TDistinct> set = new HashSet<TDistinct>();
            foreach (TValue value in source)
            {
                if (set.Add(predicate(value)))
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static T LastOr<T>(this IEnumerable<T> source, T value)
        {
            try
            {
                return source.Last();
            }
            catch (InvalidOperationException)
            {
                return value;
            }
        }

        public static unsafe void InsertionSortBy<T>(this IList<T> collection, Func<T, int> keySelector)
        {
            const int maxStackSize = 512;

            if (collection.Count <= maxStackSize)
            {
                int* keyBuffer = stackalloc int[collection.Count];
                InsertionSortBy(collection, keySelector, keyBuffer);
            }
            else
            {
                int[] keyArray = new int[collection.Count];
                fixed (int* keyBuffer = keyArray)
                {
                    InsertionSortBy(collection, keySelector, keyBuffer);
                }
            }
        }

        private static unsafe void InsertionSortBy<T>(this IList<T> collection, Func<T, int> keySelector, int* keyBuffer)
        {
            int n = collection.Count;
            for (int i = 0; i < n; i++)
            {
                keyBuffer[i] = keySelector(collection[i]);
            }

            for (int i = 1; i < n; i++)
            {
                T item = collection[i];
                int key = keyBuffer[i];
                int j = i - 1;

                while (j >= 0 && keyBuffer[j] > key)
                {
                    collection[j + 1] = collection[j];
                    keyBuffer[j + 1] = keyBuffer[j];
                    j -= 1;
                }

                collection[j + 1] = item;
                keyBuffer[j + 1] = key;
            }
        }
    }
}