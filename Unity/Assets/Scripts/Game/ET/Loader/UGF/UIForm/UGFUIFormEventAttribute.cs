using System;

namespace ET
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UGFUIFormEventAttribute : BaseAttribute
    {
        public int uiFormId { get; }

        public UGFUIFormEventAttribute(int uiFormId)
        {
            this.uiFormId = uiFormId;
            if (uiFormId == 0)
            {
                throw new Exception("UIFormId can't be 0!");
            }
        }
    }
}
