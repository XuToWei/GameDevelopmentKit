#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;

namespace TF_TableList
{
    public class ValueRowFilter : RowFilter
    {
        [ShowInInspector]
        [OnValueChanged("OnValueChanged")]
        [LabelText("@\"value of \" + PropertyName")]
        public object value;
        public override bool Success(object target)
        {
            return value.Equals(target);
        }

        public override bool IsValid => value != null;

        public ValueRowFilter(object value)
        {
            this.value = value;
            Dirty = true;
        }

        public void OnValueChanged()
        {
            Dirty = true;
        }
    }
}
#endif