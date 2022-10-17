using GameFramework.UI;
using System.Collections;
using cfg;
using cfg.UGF;
using ET;
using UnityEngine;
using UnityEngine.UI;

namespace UGF
{
    public static class UIExtension
    {
        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = alpha;
        }

        public static IEnumerator SmoothValue(this Slider slider, float value, float duration)
        {
            float time = 0f;
            float originalValue = slider.value;
            while (time < duration)
            {
                time += Time.deltaTime;
                slider.value = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }

            slider.value = value;
        }

        public static bool HasUIForm(this UnityGameFramework.Runtime.UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null)
        {
            DRUIForm drUIForm = DataTables.Instance.DTUIForm.GetOrDefault((int)uiFormId);
            if (drUIForm == null)
            {
                return false;
            }

            string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (string.IsNullOrEmpty(uiGroupName))
            {
                return uiComponent.HasUIForm(assetName);
            }

            IUIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
            if (uiGroup == null)
            {
                return false;
            }

            return uiGroup.HasUIForm(assetName);
        }

        public static UGuiForm GetUIForm(this UnityGameFramework.Runtime.UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null)
        {
            DRUIForm drUIForm = DataTables.Instance.DTUIForm.GetOrDefault((int)uiFormId);
            if (drUIForm == null)
            {
                return null;
            }

            string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
            UnityGameFramework.Runtime.UIForm uiForm = null;
            if (string.IsNullOrEmpty(uiGroupName))
            {
                uiForm = uiComponent.GetUIForm(assetName);
                if (uiForm == null)
                {
                    return null;
                }

                return (UGuiForm)uiForm.Logic;
            }

            IUIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
            if (uiGroup == null)
            {
                return null;
            }

            uiForm = (UnityGameFramework.Runtime.UIForm)uiGroup.GetUIForm(assetName);
            if (uiForm == null)
            {
                return null;
            }

            return (UGuiForm)uiForm.Logic;
        }

        public static void CloseUIForm(this UnityGameFramework.Runtime.UIComponent uiComponent, UGuiForm uiForm, object userData = null)
        {
            uiComponent.CloseUIForm(uiForm.UIForm, userData);
        }

        public static int? OpenUIForm(this UnityGameFramework.Runtime.UIComponent uiComponent, UIFormId uiFormId, object userData = null)
        {
            DRUIForm drUIForm = DataTables.Instance.DTUIForm.GetOrDefault((int)uiFormId);;
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
                return null;
            }

            string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)
            {
                if (uiComponent.IsLoadingUIForm(assetName))
                {
                    return null;
                }

                if (uiComponent.HasUIForm(assetName))
                {
                    return null;
                }
            }
            
            return uiComponent.OpenUIForm(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
        }

        public static ETTask<UnityGameFramework.Runtime.UIForm> OpenUIFormAsync(this UnityGameFramework.Runtime.UIComponent uiComponent, UIFormId uiFormId, object userData = null)
        {
            ETTask<UnityGameFramework.Runtime.UIForm> CreateNull()
            {
                ETTask<UnityGameFramework.Runtime.UIForm> task = ETTask<UnityGameFramework.Runtime.UIForm>.Create(true);
                task.SetResult(null);
                return task;
            }

            DRUIForm drUIForm = DataTables.Instance.DTUIForm.GetOrDefault((int)uiFormId);;
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
                return CreateNull();
            }

            string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)
            {
                if (uiComponent.IsLoadingUIForm(assetName))
                {
                    Log.Warning("{0} is loading!", assetName);
                    return CreateNull();
                }

                if (uiComponent.HasUIForm(assetName))
                {
                    Log.Warning("{0} already exist!", assetName);
                    return CreateNull();
                }
            }
            
            return uiComponent.OpenUIFormAsync(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
        }

        // public static void OpenDialog(this UIComponent uiComponent, DialogParams dialogParams)
        // {
        //     if (((ProcedureBase)GameEntry.Procedure.CurrentProcedure).UseNativeDialog)
        //     {
        //         OpenNativeDialog(dialogParams);
        //     }
        //     else
        //     {
        //         uiComponent.OpenUIForm(UIFormId.DialogForm, dialogParams);
        //     }
        // }
        //
        // private static void OpenNativeDialog(DialogParams dialogParams)
        // {
        //     // TODO：这里应该弹出原生对话框，先简化实现为直接按确认按钮
        //     if (dialogParams.OnClickConfirm != null)
        //     {
        //         dialogParams.OnClickConfirm(dialogParams.UserData);
        //     }
        // }
    }
}

