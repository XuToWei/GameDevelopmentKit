using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUIComponent))]
    [FriendOf(typeof(UGFUIComponent))]
    public static partial class UGFUIComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this UGFUIComponent self)
        {
            self.CloseAllUIForms();
        }

        public static async UniTask<UGFUIForm> OpenUIFormAsync(this UGFUIComponent self, int uiFormId)
        {
            return null;
        }

        public static void CloseUIForm(this UGFUIComponent self, UGFUIForm uiForm)
        {
        }

        public static void CloseUIForm(this UGFUIComponent self, int uiFormId)
        {
        }

        public static void RefocusUIForm(this UGFUIComponent self, UGFUIForm uiForm, object userData = null)
        {
        }

        public static void CloseAllUIForms(this UGFUIComponent self)
        {
        }
    }
}