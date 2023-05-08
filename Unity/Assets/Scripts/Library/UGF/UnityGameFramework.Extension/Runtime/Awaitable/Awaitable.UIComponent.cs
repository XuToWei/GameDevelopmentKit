using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        /// <summary>
        /// 打开界面（可等待）
        /// </summary>
        public static async UniTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent, string uiFormAssetName, string uiGroupName,
            int priority, bool pauseCoveredUIForm, object userData = null, CancellationToken cancellationToken = default)
        {
            int serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
            bool IsFinished()
            {
                Debug.Log((!uiComponent.IsLoadingUIForm(serialId) || cancellationToken.IsCancellationRequested) + "   "  + cancellationToken.IsCancellationRequested);
                return !uiComponent.IsLoadingUIForm(serialId) || cancellationToken.IsCancellationRequested;
            }

            try
            {
                await UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update, cancellationToken);
            }
            catch (Exception e)
            {
                throw e;
            }
            
            
            Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            if (cancellationToken.IsCancellationRequested)
            {
                uiComponent.CloseUIForm(serialId);
                return null;
            }
            return uiComponent.GetUIForm(serialId);
        }
    }
}