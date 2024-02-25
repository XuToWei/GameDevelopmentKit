#if UNITY_EDITOR && ODIN_INSPECTOR
using System;

namespace TF_TableList
{
    public abstract class TableValidator
    {
        public abstract TableValidatorResult RunValidation(object table);
    }

    public abstract class TableAttributeValidator : TableValidator
    {

    }

    public abstract class TableAttributeValidator<TAttribute> : TableAttributeValidator
    {
        public TAttribute Attribute { get; set; }
    }

    public interface IColumnValidator
    {
        string ColName { get; set; }
        Func<object, object> CellValueGetter { get; set; }
    }

    public abstract class TableColumnAttributeValidator<TAttribute> : TableAttributeValidator<TAttribute>, IColumnValidator
    {
        public string ColName { get; set; }
        public Func<object, object> CellValueGetter { get; set; }
    }
}
#endif