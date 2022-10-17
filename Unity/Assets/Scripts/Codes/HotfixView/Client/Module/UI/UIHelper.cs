using UGF;

namespace ET.Client
{
    public static partial class UIHelper
    {
        public static async ETTask<UI> Open(Scene scene, UIFormId uiFormId, object userData = null)
        {
            return await scene.GetComponent<UIComponent>().Open(uiFormId, userData);
        }
        
        public static void Close(Scene scene, UIFormId uiFormId, object userData = null)
        {
            scene.GetComponent<UIComponent>().Close(uiFormId, userData);
        }
    }
}