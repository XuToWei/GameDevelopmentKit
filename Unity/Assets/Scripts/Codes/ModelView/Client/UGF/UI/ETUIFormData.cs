using ET.Client;
using GameFramework;

namespace UGF
{
    public class ETUIFormData: IReference
    {
        public UIFormId UIFormId { get; private set; }

        public UIComponent UIComponent { get; private set; }

        public object UserData { get; private set; }

        public void Clear()
        {
            UIFormId = default;
            UIComponent = default;
            UserData = default;
        }

        public void Fill(UIFormId uiFormId, UIComponent uiComponent, object userData)
        {
            this.UIFormId = uiFormId;
            this.UIComponent = uiComponent;
            this.UserData = userData;
        }
    }
}