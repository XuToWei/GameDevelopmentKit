using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        private static readonly Dictionary<int, OpenUIFormEventData> s_OpenUIFormEventDataDict = new Dictionary<int, OpenUIFormEventData>();
        
        /// <summary>
        /// 打开界面（可等待）
        /// </summary>
        public static UniTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent, string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, 
            object userData = null, CancellationToken cancellationToken = default, Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<UIForm>(cancellationToken);
            }
            OpenUIFormEventData eventData = ReferencePool.Acquire<OpenUIFormEventData>();
            eventData.UpdateEvent = updateEvent;
            eventData.IsError = false;
            eventData.ErrorMessage = null;
            eventData.DependencyAssetEvent = dependencyAssetEvent;

            int serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
            s_OpenUIFormEventDataDict.Add(serialId, eventData);

            bool delayOneFrame = true;
            bool MoveNext(ref UniTaskCompletionSourceCore<UIForm> core)
            {
                if (!IsValid)
                {
                    core.TrySetCanceled();
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    uiComponent.CloseUIForm(serialId);
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }
                if (uiComponent.IsLoadingUIForm(serialId))
                {
                    return true;
                }
                UIForm uiForm = uiComponent.GetUIForm(serialId);
                if (uiForm == null)//这里是被其他接口关闭了
                {
                    if (delayOneFrame)//等待一帧GF的Event.Fire，确保能接收到错误的事件处理后继续（PlayerLoopTiming.LastUpdate）
                    {
                        delayOneFrame = false;
                        return true;
                    }
                    if (eventData.IsError)
                    {
                        core.TrySetException(new GameFrameworkException(eventData.ErrorMessage));
                    }
                    else
                    {
                        core.TrySetCanceled();
                    }
                }
                else
                {
                    core.TrySetResult(uiForm);
                }
                return false;
            }

            void ReturnAction()
            {
                s_OpenUIFormEventDataDict.Remove(serialId);
                ReferencePool.Release(eventData);
            }
            return NewUniTask<UIForm>(MoveNext, cancellationToken, ReturnAction);
        }
        
        private class OpenUIFormEventData : IReference
        {
            public Action<float> UpdateEvent;
            public bool IsError;
            public string ErrorMessage;
            public Action<string> DependencyAssetEvent;

            public void Clear()
            {
                UpdateEvent = null;
                IsError = false;
                ErrorMessage = null;
                DependencyAssetEvent = null;
            }
        }

        private static void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if(s_OpenUIFormEventDataDict.TryGetValue(ne.UIForm.SerialId, out OpenUIFormEventData eventData))
            {
                eventData.IsError = false;
            }
        }

        private static void OnOpenUIFormFailure(object sender, GameEventArgs e)
        {
            OpenUIFormFailureEventArgs ne = (OpenUIFormFailureEventArgs)e;
            if(s_OpenUIFormEventDataDict.TryGetValue(ne.SerialId, out OpenUIFormEventData eventData))
            {
                eventData.IsError = true;
                eventData.ErrorMessage = ne.ErrorMessage;
            }
        }
        
        private static void OnOpenUIFormUpdate(object sender, GameEventArgs e)
        {
            OpenUIFormUpdateEventArgs ne = (OpenUIFormUpdateEventArgs)e;
            if(s_OpenUIFormEventDataDict.TryGetValue(ne.SerialId, out OpenUIFormEventData eventData))
            {
                eventData.UpdateEvent?.Invoke(ne.Progress);
            }
        }
        
        private static void OnOpenUIFormDependencyAsset(object sender, GameEventArgs e)
        {
            OpenUIFormDependencyAssetEventArgs ne = (OpenUIFormDependencyAssetEventArgs)e;
            if(s_OpenUIFormEventDataDict.TryGetValue(ne.SerialId, out OpenUIFormEventData eventData))
            {
                eventData.DependencyAssetEvent?.Invoke(ne.DependencyAssetName);
            }
        }
    }
}
