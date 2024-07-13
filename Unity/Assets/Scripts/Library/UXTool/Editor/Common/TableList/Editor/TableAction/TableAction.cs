#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TF_TableList
{
    public class TableActionContext
    {
        public object targetTable;
        public ExtraTableInfo extraInfo;
        public IEnumerable SelectedRows()
        {
            if (targetTable is IList obj)
            {
                foreach (var rowExtraInfo in extraInfo.ReOrderedRows)
                {
                    if (rowExtraInfo.selected)
                    {
                        yield return obj[rowExtraInfo.origIndex];
                    }
                }
            }
        }
    }

    public class TableAction
    {

        public string TargetColumn { get; set; }
        public string Name { get; set; }
        public Texture Icon { get; set; }
        public TableActionPlace Place { get; set; }
        public string MemberName { get; set; }
        public object Parent { get; set; }
        public string ErrorMessage;
        public MethodInfo methodInfo;

        public bool IsActive(TableActionContext context)
        {
            return true;
        }

        public void DoAction(TableActionContext context)
        {
            methodInfo.Invoke(Parent, new object[] { context });
        }
    }

    public class TableRowSelectContext
    {
        public object table;
        public object row;
    }

    public class TableRowSelectCallBack
    {
        public string MemberName { get; set; }
        public object Parent { get; set; }
        public string ErrorMessage;
        public bool Delayed;
        public MethodInfo methodInfo;

        public void DoAction(TableRowSelectContext row)
        {
            if (Delayed)
            {
                EditorApplication.delayCall += () =>
                {
                    methodInfo.Invoke(Parent, new object[] { row });
                };
            }
            else
            {
                methodInfo.Invoke(Parent, new object[] { row });
            }
        }
    }
}
#endif