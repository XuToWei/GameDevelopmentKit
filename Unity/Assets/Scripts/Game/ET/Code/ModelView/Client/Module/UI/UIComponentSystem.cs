using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIComponent))]
    [FriendOf(typeof(UIComponent))]
    public static partial class UIComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this UIComponent self)
        {
            self.CloseAllUIForms();
        }

        public static async UniTask<UGFUIForm> OpenUIFormAsync(this UIComponent self, int uiFormId)
        {
            return null;
        }

        public static void CloseUIForm(this UIComponent self, UGFUIForm uiForm)
        {
        }

        public static void CloseUIForm(this UIComponent self, int uiFormId)
        {
        }

        public static void RefocusUIForm(this UIComponent self, UGFUIForm uiForm, object userData = null)
        {
        }

        public static void CloseAllUIForms(this UIComponent self)
        {
        }
    }
}