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
            eventData.DependencyAssetEvent = dependencyAssetEvent;

            int serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
            s_OpenUIFormEventDataDict.Add(serialId, eventData);

            bool MoveNext(ref UniTaskCompletionSourceCore<UIForm> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    if (uiComponent.HasUIForm(serialId))
                    {
                        uiComponent.CloseUIForm(serialId);
                    }
                    return false;
                }
                if (uiComponent.IsLoadingUIForm(serialId))
                {
                    return true;
                }
                UIForm uiForm = uiComponent.GetUIForm(serialId);
                if (uiForm == null)//这里是被其他接口关闭了
                {
                    core.TrySetException(new GameFrameworkException(Utility.Text.Format("Open UI form task is failure, asset name '{0}', UI group name '{1}', pause covered UI form '{2}'.", uiFormAssetName, uiGroupName, pauseCoveredUIForm)));
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

        private sealed class OpenUIFormEventData : IReference
        {
            public Action<float> UpdateEvent;
            public Action<string> DependencyAssetEvent;

            public void Clear()
            {
                UpdateEvent = null;
                DependencyAssetEvent = null;
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
