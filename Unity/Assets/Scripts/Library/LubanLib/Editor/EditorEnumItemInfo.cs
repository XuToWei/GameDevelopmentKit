using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban
{
    public class EditorEnumItemInfo
    {
        public string Name { get; }

        public string Alias { get; }

        public int Value { get; }

        public string Comment { get; }

        public EditorEnumItemInfo(string name, string alias, int value, string comment)
        {
            Name = name;
            Alias = alias;
            Value = value;
            Comment = comment;
        }
    }
}
