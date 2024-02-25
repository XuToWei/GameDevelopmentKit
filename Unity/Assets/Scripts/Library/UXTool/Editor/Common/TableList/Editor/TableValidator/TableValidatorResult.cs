#if UNITY_EDITOR && ODIN_INSPECTOR
using System.Collections.Generic;
using Sirenix.Utilities;

namespace TF_TableList
{
    public class TableValidatorResult
    {
        public DoubleLookupDictionary<int, string, List<string>> InvalidCells = new DoubleLookupDictionary<int, string, List<string>>();
        public int ErrorCount = 0;

        public void Reset()
        {
            InvalidCells.Clear();
            ErrorCount = 0;
        }

        public void Merge(TableValidatorResult other)
        {
            foreach (var rowInfo in other.InvalidCells)
            {
                foreach (var cellInfo in rowInfo.Value)
                {
                    var dict = InvalidCells[rowInfo.Key];
                    if (!dict.TryGetValue(cellInfo.Key, out var list))
                    {
                        dict[cellInfo.Key] = new List<string>();
                    }
                    dict[cellInfo.Key].AddRange(cellInfo.Value);
                }
            }
        }

        public void RefreshErrorCount()
        {
            ErrorCount = 0;
            foreach (var kv in InvalidCells)
            {
                foreach (var kv2 in kv.Value)
                {
                    ErrorCount += kv2.Value.Count;
                }
            }
        }
    }
}
#endif