using System;

namespace ET
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class UGFUIFormEventAttribute : BaseAttribute
    {
        public int[] uiFormIds { get; }

        public UGFUIFormEventAttribute(params int[] uiFormIds)
        {
            this.uiFormIds = uiFormIds;
#if UNITY_EDITOR
            if (this.uiFormIds == null || this.uiFormIds.Length < 1)
            {
                throw new Exception("UIFormIds can't be null!");
            }
            foreach (var uiFormId in uiFormIds)
            {
                if (uiFormId == 0)
                {
                    throw new Exception("UIFormId can't be 0!");
                }
            }
#endif
        }
    }
}
