using System;
using System.Collections.Generic;

namespace UnityGameFramework.Extension
{
    public class MultiMap<T, K>: SortedDictionary<T, List<K>>
    {
        private readonly List<K> m_Empty = new List<K>();

        public void Add(T t, K k)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                list = new List<K>();
                Add(t, list);
            }
            list.Add(k);
        }

        public bool Remove(T t, K k)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                Remove(t);
            }
            return true;
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                return Array.Empty<K>();
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new List<K> this[T t]
        {
            get
            {
                TryGetValue(t, out List<K> list);
                return list ?? m_Empty;
            }
        }

        public K GetOne(T t)
        {
            TryGetValue(t, out var list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }

        public bool Contains(T t, K k)
        {
            TryGetValue(t, out var list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }
    }
}