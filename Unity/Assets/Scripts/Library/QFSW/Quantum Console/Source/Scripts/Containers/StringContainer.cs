using System.Collections;
using System.Collections.Generic;

namespace QFSW.QC.Containers
{
    public struct StringContainer : IReadOnlyList<char>
    {
        private readonly string _str;

        public StringContainer(string str)
        {
            _str = str;
        }

        public char this[int index] => _str[index];

        public int Count => _str.Length;

        public IEnumerator<char> GetEnumerator()
        {
            for (int i = 0; i < _str.Length; i++)
            {
                yield return _str[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _str.Length; i++)
            {
                yield return _str[i];
            }
        }

        public static implicit operator StringContainer(string str)
        {
            return new StringContainer(str);
        }

        public static implicit operator string(StringContainer str)
        {
            return str._str;
        }
    }

    public static class StringContainerExtensions
    {
        public static StringContainer AsIReadOnlyList(this string str)
        {
            return str;
        }
    }
}
