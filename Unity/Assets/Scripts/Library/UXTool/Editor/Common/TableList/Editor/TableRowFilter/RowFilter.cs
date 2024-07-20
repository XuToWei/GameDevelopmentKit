#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using UnityEngine;

namespace TF_TableList
{
    public enum FilterConditionMode
    {
        And,
        Or,
    }

    public enum RowFilterTargetType
    {
        Row,
        Cell,
    }

    public abstract class RowFilter : ITablePersistentComponent
    {
        public string Name { get; set; } = "RowFilter";
        public string PropertyName { get; set; }
        public RowFilterTargetType TargetType { get; set; } = RowFilterTargetType.Cell;
        public bool Dirty { get; set; } = false;
        public virtual bool IsEnable { get; set; } = true;
        public virtual bool IsValid { get; } = true;
        public abstract bool Success(object target);
        public Func<object, object> CellValueGetter;

        public bool SuccessRow(object row)
        {
            if (TargetType == RowFilterTargetType.Row)
            {
                return Success(row);
            }
            else if (TargetType == RowFilterTargetType.Cell && CellValueGetter != null)
            {
                return Success(CellValueGetter(row));
            }
            return false;
        }

        public virtual void Initialize()
        {

        }

        public bool UseDrawProperty { get; } = false;
        public virtual void DrawProperty()
        {

        }


        public virtual void Reset()
        {
        }

        public virtual void Prepare(object table)
        {
        }
        public virtual bool Deletable { get; set; } = true;
        public virtual bool Inline { get; set; } = false;
        public virtual string PersistentContextKey => $"Filter_{this.GetType().Name}_{PropertyName}_{Inline}";
        public virtual string GetPersistentValue() => null;
        public virtual void OnLoadPersistentValue(string value) { }
        public virtual void OnDrawInline(Rect rect) { }
    }

    public abstract class RowFilter<T> : RowFilter
    {
        public override bool Success(object target)
        {
            if (target is T st)
            {
                return Success(st);
            }
            return false;
        }

        public abstract bool Success(T target);
    }
}
#endif