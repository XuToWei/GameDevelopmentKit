using UGF;

namespace ET.Client
{
    public class UIEventAttribute: BaseAttribute
    {
        public UIFormId UIFormId { get; }

        public UIEventAttribute(UIFormId uiFormId)
        {
            this.UIFormId = uiFormId;
        }
    }
}