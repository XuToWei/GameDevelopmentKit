using System.Collections;
using System.Collections.Generic;

namespace QFSW.QC.Containers
{
    public struct ArraySingle<T> : IReadOnlyList<T>
    {
        private readonly T _data;

        public ArraySingle(T data)
        {
            _data = data;
        }

        public T this[int index] => _data;

        public int Count => 1;

        public IEnumerator<T> GetEnumerator()
        {
            yield return _data;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return _data;
        }
    }

    public static class ArraySingleExtensions
    {
        public static ArraySingle<T> AsArraySingle<T>(this T data)
        {
            return new ArraySingle<T>(data);
        }
    }
}
