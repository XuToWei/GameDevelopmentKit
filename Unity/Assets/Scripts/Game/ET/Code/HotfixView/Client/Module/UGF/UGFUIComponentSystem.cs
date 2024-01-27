using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFUIComponent))]
    [FriendOf(typeof(UGFUIComponent))]
    public static partial class UGFUIComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFUIComponent self)
        {
            self.AllOpenUIForms.Clear();
        }

        [EntitySystem]
        private static void Destroy(this UGFUIComponent self)
        {
            self.CloseAllUIForms();
        }

        public static async UniTask<UGFUIForm> OpenUIFormAsync(this UGFUIComponent self, int uiFormId, object userData = null)
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
                throw new Exception($"Open UI fail! UIFomId:{uiFormId}) is not ETMonoUIForm!");
            }
            return etMonoUIForm.UGFUIForm;
        }

        public static void CloseUIForm(this UGFUIComponent self, UGFUIForm uiForm)
        {
            if (!self.AllOpenUIForms.Contains(uiForm))
                return;
            GameEntry.UI.CloseUIForm(uiForm.UIForm);
        }

        public static void CloseUIForm(this UGFUIComponent self, int uiFormId)
        {
            using HashSetComponent<UGFUIForm> needRemoves = new HashSetComponent<UGFUIForm>();
            foreach (UGFUIForm uiForm in self.AllOpenUIForms)
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

        public static void RefocusUIForm(this UGFUIComponent self, UGFUIForm uiForm, object userData = null)
        {
            if (!self.AllOpenUIForms.Contains(uiForm))
                return;
            GameEntry.UI.RefocusUIForm(uiForm.UIForm, userData);
        }

        public static void CloseAllUIForms(this UGFUIComponent self)
        {
            foreach (UGFUIForm uiForm in self.AllOpenUIForms.ToArray())
            {
                if (uiForm != null)
                {
                    self.CloseUIForm(uiForm);
                }
            }
            self.AllOpenUIForms.Clear();
        }
    }
}