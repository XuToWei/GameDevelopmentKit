using System.Collections;
using System;
using System.Collections.Generic;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ValueDropdownAttribute : DrawerAttribute
    {
        public string ValuesName { get; private set; }

        public ValueDropdownAttribute(string valuesName)
        {
            ValuesName = valuesName;
        }
    }

    public interface IDropdownList : IEnumerable<KeyValuePair<string, object>>
    {
    }

    public class ValueDropdownList<T> : IDropdownList
    {
        private List<KeyValuePair<string, object>> _values;

        public ValueDropdownList()
        {
            _values = new List<KeyValuePair<string, object>>();
        }

        public void Add(string displayName, T value)
        {
            _values.Add(new KeyValuePair<string, object>(displayName, value));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static explicit operator ValueDropdownList<object>(ValueDropdownList<T> target)
        {
            ValueDropdownList<object> result = new ValueDropdownList<object>();
            foreach (var kvp in target)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }
    }
}
