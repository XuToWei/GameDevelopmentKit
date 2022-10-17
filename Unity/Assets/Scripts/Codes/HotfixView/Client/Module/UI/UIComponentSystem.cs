using System;
using System.Collections.Generic;
using GameFramework;
using UGF;
using UnityGameFramework.Runtime;
using GameEntry = UGF.GameEntry;

namespace ET.Client
{
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [FriendOf(typeof (UIComponent))]
    public static class UIComponentSystem
    {
        [ObjectSystem]
        public class UIComponentAwakeSystem: AwakeSystem<UIComponent>
        {
            protected override void Awake(UIComponent self)
            {
                
            }
        }

        public static async ETTask<UI> Open(this UIComponent self, UIFormId uiFormId, object userData = null)
        {
            ETUIFormData formData = ReferencePool.Acquire<ETUIFormData>();
            formData.Fill(uiFormId, self, userData);
            UIForm uiForm = await GameEntry.UI.OpenUIFormAsync(uiFormId, formData);
            if (uiForm == null)
            {
                ReferencePool.Release(formData);
                Log.Warning($"{uiFormId} open fail!");
                return null;
            }
            ETUIForm etUIForm = uiForm.Logic as ETUIForm;
            if (etUIForm == null)
            {
                throw new Exception($"{uiForm.UIFormAssetName} is not ETUIForm!");
            }
            if (self.UIs.TryGetValue(uiFormId, out List<UI> uis))
            {
                uis.Add(etUIForm.UI);
            }
            else
            {
                self.UIs.Add(uiFormId, new List<UI>() { etUIForm.UI });
            }
            return etUIForm.UI;
        }

        public static void Close(this UIComponent self, UIFormId uiFormId, object userData = null)
        {
            if (!self.UIs.TryGetValue(uiFormId, out List<UI> uis))
            {
                return;
            }
            self.UIs.Remove(uiFormId);
            
            foreach (UI ui in uis)
            {
                GameEntry.UI.CloseUIForm(ui.UIForm, userData);
            }
        }

        public static void Close(this UIComponent self, UI ui, object userData = null)
        {
            UIFormId uiFormId = ui.UIFormId;
            if (self.UIs.TryGetValue(uiFormId, out List<UI> uis))
            {
                uis.Remove(ui);
            }

            GameEntry.UI.CloseUIForm(ui.UIForm, userData);
        }

        public static UI GetOne(this UIComponent self, UIFormId uiFormId)
        {
            if (self.UIs.TryGetValue(uiFormId, out List<UI> ui))
            {
                return ui[0];
            }

            return null;
        }

        public static List<UI> GetAll(this UIComponent self, UIFormId uiFormId)
        {
            List<UI> ui = null;
            self.UIs.TryGetValue(uiFormId, out ui);
            return ui;
        }
    }
}