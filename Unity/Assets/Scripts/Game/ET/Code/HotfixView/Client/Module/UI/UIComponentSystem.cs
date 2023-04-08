using System;
using Cysharp.Threading.Tasks;
using Game;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET.Client
{
    [FriendOf(typeof (UIComponent))]
    public static class UIComponentSystem
    {
        [ObjectSystem]
        public class UIComponentAwakeSystem: AwakeSystem<UIComponent>
        {
            protected override void Awake(UIComponent self)
            {
                UIComponent.Instance = self;
            }
        }

        [ObjectSystem]
        public class UIComponentDestroySystem: DestroySystem<UIComponent>
        {
            protected override void Destroy(UIComponent self)
            {
                UIComponent.Instance = null;
            }
        }

        public static async UniTask<UGFUIForm> OpenUIFormAsync(this UIComponent self, int uiFormId, object userData = null)
        {
            ETMonoUIFormData formData = ETMonoUIFormData.Acquire(uiFormId, self, userData);
            UIForm uiForm = await GameEntry.UI.OpenUIFormAsync(uiFormId, formData);
            if (uiForm == null)
            {
                formData.Release();
                return null;
            }
            if (uiForm.Logic is not ETMonoUIForm etMonoUIForm)
            {
                throw new Exception($"Open UI fail! UiFomId:{uiFormId}) is not ETMonoUIForm!");
            }
            return etMonoUIForm.ugfUIForm;
        }

        public static void CloseUIForm(this UIComponent self, UGFUIForm uiForm)
        {
            GameEntry.UI.CloseUIForm(uiForm.etMonoUIForm.UIForm);
        }

        public static void CloseUIForm(this UIComponent self, int uiFormId)
        {
            using HashSetComponent<UGFUIForm> needRemoves = new HashSetComponent<UGFUIForm>();
            foreach (UGFUIForm uiForm in self.AllOpenUIForms)
            {
                if (uiForm.uiFormId == uiFormId)
                {
                    needRemoves.Add(uiForm);
                }
            }

            foreach (UGFUIForm uiForm in needRemoves)
            {
                self.CloseUIForm(uiForm);
            }
        }

        public static void RefocusUIForm(this UIComponent self, UGFUIForm uiForm, object userData = null)
        {
            GameEntry.UI.RefocusUIForm(uiForm.etMonoUIForm.UIForm, userData);
        }
    }
}