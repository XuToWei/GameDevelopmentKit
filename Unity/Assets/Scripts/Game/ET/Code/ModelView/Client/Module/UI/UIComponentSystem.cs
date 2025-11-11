using System;
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [FriendOf(typeof(UIComponent))]
    [EntitySystemOf(typeof(UIComponent))]
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

        public static async UniTask<T> AddUIFormChildAsync<T>(this UIComponent self, int uiFormTypeId, bool isFromPool = false) where T : UGFUIForm, IAwake
        {
            T ugfUIForm = self.AddChild<T>(isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
        }
        public static async UniTask<T> AddUIFormChildAsync<T, A>(this UIComponent self, int uiFormTypeId, A a, bool isFromPool = false) where T : UGFUIForm, IAwake<A>
        {
            T ugfUIForm = self.AddChild<T, A>(a, isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
        }
        
        public static async UniTask<T> AddUIFormChildAsync<T, A, B>(this UIComponent self, int uiFormTypeId, A a, B b, bool isFromPool = false) where T : UGFUIForm, IAwake<A, B>
        {
            T ugfUIForm = self.AddChild<T, A, B>(a, b, isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
        }

        public static async UniTask<T> AddUIFormChildAsync<T, A, B, C>(this UIComponent self, int uiFormTypeId, A a, B b, C c, bool isFromPool = false) where T : UGFUIForm, IAwake<A, B, C>
        {
            T ugfUIForm = self.AddChild<T, A, B, C>(a, b, c, isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
        }

        public static async UniTask<T> AddUIFormComponentAsync<T>(this UIComponent self, int uiFormTypeId, bool isFromPool = false) where T : UGFUIForm, IAwake, new()
        {
            T ugfUIForm = self.AddComponent<T>(isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
        }
        public static async UniTask<T> AddUIFormComponentAsync<T, A>(this UIComponent self, int uiFormTypeId, A a, bool isFromPool = false) where T : UGFUIForm, IAwake<A>, new()
        {
            T ugfUIForm = self.AddComponent<T, A>(a, isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
        }

        public static async UniTask<T> AddUIFormComponentAsync<T, A, B>(this UIComponent self, int uiFormTypeId, A a, B b, bool isFromPool = false) where T : UGFUIForm, IAwake<A, B>, new()
        {
            T ugfUIForm = self.AddComponent<T, A, B>(a, b, isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
        }

        public static async UniTask<T> AddUIFormComponentAsync<T, A, B, C>(this UIComponent self, int uiFormTypeId, A a, B b, C c, bool isFromPool = false) where T : UGFUIForm, IAwake<A, B, C>, new()
        {
            T ugfUIForm = self.AddComponent<T, A, B, C>(a, b, c, isFromPool);
            await ugfUIForm.OpenUIFormAsync(uiFormTypeId);
            return ugfUIForm;
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

            using var removeComponentTypes = ListComponent<Type>.Create();
            foreach (var component in self.Components.Values)
            {
                if (component is UGFUIForm)
                {
                    removeComponentTypes.Add(component.GetType());
                }
            }
            foreach (var componentType in removeComponentTypes)
            {
                self.RemoveComponent(componentType);
            }
        }
    }
}