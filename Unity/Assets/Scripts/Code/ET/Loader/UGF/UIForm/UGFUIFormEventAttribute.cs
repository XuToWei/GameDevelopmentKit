namespace ET
{
    public sealed class UGFUIFormEventAttribute : BaseAttribute
    {
        public int UIFormId { get; }

        public UGFUIFormEventAttribute(int uiFormId)
        {
            this.UIFormId = uiFormId;
        }
    }
}
