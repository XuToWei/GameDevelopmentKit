#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;

namespace TF_TableList
{
    public class BooleanRowFilter : RowFilter
    {
        [ShowInInspector]
        [OnValueChanged("OnValueChanged")]
        [LabelText("@\"filter by \" + PropertyName")]
        public bool value = true;
        public override bool Success(object target)
        {
            return value.Equals(target);
        }

        public override bool IsValid => IsEnable;

        public override bool IsEnable { get; set; } = false;

        public BooleanRowFilter()
        {
        }

        public void OnValueChanged()
        {
            Dirty = true;
        }
    }
}
#endif