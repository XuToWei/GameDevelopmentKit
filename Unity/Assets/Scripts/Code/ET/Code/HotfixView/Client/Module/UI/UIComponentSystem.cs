using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;
using Game;
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

        public static async ETTask<UGFUIForm> OpenUIFormAsync(this UIComponent self, int uiFormId, object userData = null)
        {
            ETMonoUIFormData formData = ETMonoUIFormData.Acquire(uiFormId, self, userData);
            UIForm uiForm = await GameEntry.UI.OpenUIFormAsync(uiFormId, formData);
            if (uiForm == null)
                return null;
            if (uiForm.Logic is not ETMonoUIForm etMonoUIForm)
            {
                throw new Exception($"Open UI fail! UiFomId:{uiFormId}) is not ETMonoUIForm!");
            }

            formData.Release();
            return etMonoUIForm.UGFUIForm;
        }

        public static void CloseUIForm(this UIComponent self, UGFUIForm uiForm)
        {
            GameEntry.UI.CloseUIForm(uiForm.EtMonoUIForm.UIForm);
        }

        public static void CloseUIForm(this UIComponent self, int uiFormId)
        {
            HashSet<UGFUIForm> needRemoves = new HashSet<UGFUIForm>();
            foreach (UGFUIForm uiForm in self.UIForms)
            {
                if (uiForm.UIFormId == uiFormId)
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
            GameEntry.UI.RefocusUIForm(uiForm.EtMonoUIForm.UIForm, userData);
        }
    }
}