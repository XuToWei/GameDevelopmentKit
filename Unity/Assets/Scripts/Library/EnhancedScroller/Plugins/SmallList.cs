using UnityEngine;
using System.Collections.Generic;

namespace EnhancedUI
{
    /// <summary>
    /// This is a super light implementation of an array that
    /// behaves like a list, automatically allocating new memory
    /// when needed, but not releasing it to garbage collection.
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    public class SmallList<T>
    {
        /// <summary>
        /// internal storage of list data
        /// </summary>
        public T[] data;

        /// <summary>
        /// The number of elements in the list
        /// </summary>
        public int Count = 0;

        /// <summary>
        /// Indexed access to the list items
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i]
        {
            get
            {
                if (data is null)
                {
                    return default(T);
                }
                else
                {
                    return data[i];
                }
            }

            set { data[i] = value; }
        }

        /// <summary>
        /// Resizes the array when more memory is needed.
        /// </summary>
        private void ResizeArray()
        {
            T[] newData;

            if (data != null)
                newData = new T[Mathf.Max(data.Length << 1, 64)];
            else
                newData = new T[64];

            if (data != null && Count > 0)
                data.CopyTo(newData, 0);

            data = newData;
        }

        /// <summary>
        /// Instead of releasing the memory to garbage collection,
        /// the list size is set back to zero
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Returns the first element of the list
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            if (data == null || Count == 0) return default(T);
            return data[0];
        }

        /// <summary>
        /// Returns the last element of the list
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            if (data == null || Count == 0) return default(T);
            return data[Count - 1];
        }

        /// <summary>
        /// Adds a new element to the array, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (data == null || Count == data.Length)
                ResizeArray();

            data[Count] = item;
            Count++;
        }

        /// <summary>
        /// Adds a new element to the start of the array, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void AddStart(T item)
        {
            Insert(item, 0);
        }

        /// <summary>
        /// Inserts a new element to the array at the index specified, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item, int index)
        {
            if (data == null || Count == data.Length)
                ResizeArray();

            for (var i = Count; i > index; i--)
            {
                data[i] = data[i - 1];
            }

            data[index] = item;
            Count++;
        }

        /// <summary>
        /// Removes an item from the start of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveStart()
        {
            return RemoveAt(0);
        }

        /// <summary>
        /// Removes an item from the index of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveAt(int index)
        {
            if (data != null && Count != 0)
            {
                T val = data[index];

                for (var i = index; i < Count - 1; i++)
                {
                    data[i] = data[i + 1];
                }

                Count--;
                data[Count] = default(T);
                return val;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Removes an item from the data
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Remove(T item)
        {
            if (data != null && Count != 0)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (data[i].Equals(item))
                    {
                        return RemoveAt(i);
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// Removes an item from the end of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveEnd()
        {
            if (data != null && Count != 0)
            {
                Count--;
                T val = data[Count];
                data[Count] = default(T);

                return val;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Determines if the data contains the item
        /// </summary>
        /// <param name="item">The item to compare</param>
        /// <returns>True if the item exists in teh data</returns>
        public bool Contains(T item)
        {
            if (data == null)
                return false;

            for (var i = 0; i < Count; i++)
            {
                if (data[i].Equals(item))
                    return true;
            }

            return false;
        }
    }
}
