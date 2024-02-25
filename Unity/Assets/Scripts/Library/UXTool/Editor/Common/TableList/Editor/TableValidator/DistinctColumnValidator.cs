#if UNITY_EDITOR && ODIN_INSPECTOR
using System.Collections;
using System.Collections.Generic;

namespace TF_TableList
{
    public class DistinctColumnValidatorAttribute : TableValidatorAttribute
    {
        public DistinctColumnValidatorAttribute() : base(typeof(DistinctColumnValidator))
        {
        }
    }

    public class DistinctColumnValidator : TableColumnAttributeValidator<DistinctColumnValidatorAttribute>, IColumnValidator
    {
        private TableValidatorResult result = new TableValidatorResult();
        public const string ErrorMessage = "values in this column should be distinct";

        public override TableValidatorResult RunValidation(object table)
        {
            result.Reset();
            var list = table as IList;
            if (list == null || list.Count == 0)
                return result;
            if (CellValueGetter == null)
                return result;
            Dictionary<object, int> dict = new Dictionary<object, int>();
            HashSet<int> errorIndexSet = new HashSet<int>();
            for (int i = 0; i < list.Count; i++)
            {
                var value = CellValueGetter(list[i]);
                if (dict.TryGetValue(value, out var origId))
                {
                    errorIndexSet.Add(origId);
                    errorIndexSet.Add(i);
                }
                else
                {
                    dict.Add(value, i);
                }
            }
            foreach (var rowIndex in errorIndexSet)
            {
                if (!result.InvalidCells.ContainsKeys(rowIndex, ColName))
                {
                    result.InvalidCells[rowIndex][ColName] = new List<string>();
                }
                result.InvalidCells[rowIndex][ColName].Add(ErrorMessage);
            }
            return result;
        }
    }
}
#endif