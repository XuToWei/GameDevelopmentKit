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
            
        }
        
        public static async UniTask<T> AddUIFormAsync<T>(this UIComponent self, int uiFormTypeId) where T : UGFUIForm, new()
        {
            T ugfUIForm = self.AddChild<T>();
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
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
            using var removeChildIds = ListComponent<long>.Create();
            foreach (var child in self.Children.Values)
            {
                if (child is UGFUIForm)
                {
                    removeChildIds.Add(child.Id);
                }
            }
            foreach (var childId in removeChildIds)
            {
                self.RemoveChild(childId);
            }
        }
    }
}