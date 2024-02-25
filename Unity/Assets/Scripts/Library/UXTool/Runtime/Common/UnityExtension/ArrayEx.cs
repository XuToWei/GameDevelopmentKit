using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThunderFireUnityEx
{

    public static class ArrayEx
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            length = Mathf.Clamp(length, 0, data.Length - index);
            T[] result = new T[length];
            if (length > 0)
                Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static T[] InsertAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length + 1];
            index = Mathf.Clamp(index, 0, source.Length - 1);

            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            Array.Copy(source, index, dest, index + 1, source.Length - index);

            return dest;
        }

        public static T[] Swap<T>(this T[] source, int index, int with)
        {
            index = Mathf.Clamp(index, 0, source.Length - 1);
            with = Mathf.Clamp(index, 0, source.Length - 1);
            T tmp = source[index];
            source[index] = source[with];
            source[with] = tmp;
            return source;
        }

        public static T[] Add<T>(this T[] source, T item)
        {
            Array.Resize<T>(ref source, source.Length + 1);
            source[source.Length - 1] = item;
            return source;
        }

        public static T[] AddRange<T>(this T[] source, T[] items)
        {
            Array.Resize<T>(ref source, source.Length + items.Length);
            System.Array.Copy(items, 0, source, source.Length - items.Length, items.Length);
            return source;
        }

        public static T[] RemoveDuplicates<T>(this T[] source)
        {
            List<T> res = new List<T>();
            HashSet<T> hash = new HashSet<T>();
            foreach (var p in source)
            {
                if (hash.Add(p))
                {
                    res.Add(p);
                }
            }
            return res.ToArray();
        }

        public static int IndexOf<T>(this T[] source, T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < source.Length; i++)
                if (comparer.Equals(source[i], item))
                    return i;
            return -1;
        }

        public static T[] Remove<T>(this T[] source, T item)
        {
            int idx = source.IndexOf<T>(item);
            if (idx > -1)
                return source.RemoveAt<T>(idx);
            else
                return source;
        }
    }
}