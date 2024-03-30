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
        private static readonly Dictionary<int, ShowEntityEventData> s_ShowEntityEventDataDict = new Dictionary<int, ShowEntityEventData>();
        
         /// <summary>
        /// 显示实体（可等待）
        /// </summary>
        public static UniTask<Entity> ShowEntityAsync(this EntityComponent entityComponent, int entityId, Type entityLogicType, string entityAssetName, string entityGroupName,
             int priority = 0, object userData = null, CancellationToken cancellationToken = default, Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (cancellationToken.IsCancellationRequested)
            {
                return UniTask.FromCanceled<Entity>(cancellationToken);
            }
            ShowEntityEventData eventData = ReferencePool.Acquire<ShowEntityEventData>();
            eventData.UpdateEvent = updateEvent;
            eventData.IsError = false;
            eventData.ErrorMessage = null;
            eventData.DependencyAssetEvent = dependencyAssetEvent;
            
            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            s_ShowEntityEventDataDict.Add(entityId, eventData);

            bool delayOneFrame = true;
            bool MoveNext(ref UniTaskCompletionSourceCore<Entity> core)
            {
                if (!IsValid)
                {
                    core.TrySetCanceled();
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    entityComponent.HideEntity(entityId);
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }
                if (entityComponent.IsLoadingEntity(entityId))
                {
                    return true;
                }
                Entity entity = entityComponent.GetEntity(entityId);
                if (entity == null)//这里是被其他接口关闭了
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
                    core.TrySetResult(entity);
                }
                return false;
            }
            
            void ReturnAction()
            {
                s_ShowEntityEventDataDict.Remove(entityId);
                ReferencePool.Release(eventData);
            }

            return NewUniTask<Entity>(MoveNext, cancellationToken, ReturnAction);
        }
         
         private class ShowEntityEventData : IReference
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

        private static void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (s_ShowEntityEventDataDict.TryGetValue(ne.Entity.Id, out ShowEntityEventData eventData))
            {
                eventData.IsError = false;
            }
        }

        private static void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            if (s_ShowEntityEventDataDict.TryGetValue(ne.EntityId, out ShowEntityEventData eventData))
            {
                eventData.IsError = true;
                eventData.ErrorMessage = ne.ErrorMessage;
            }
        }

        private static void OnShowEntityUpdate(object sender, GameEventArgs e)
        {
            ShowEntityUpdateEventArgs ne = (ShowEntityUpdateEventArgs)e;
            if (s_ShowEntityEventDataDict.TryGetValue(ne.EntityId, out ShowEntityEventData eventData))
            {
                eventData.UpdateEvent?.Invoke(ne.Progress);
            }
        }
        
        private static void OnShowEntityDependencyAsset(object sender, GameEventArgs e)
        {
            ShowEntityDependencyAssetEventArgs ne = (ShowEntityDependencyAssetEventArgs)e;
            if (s_ShowEntityEventDataDict.TryGetValue(ne.EntityId, out ShowEntityEventData eventData))
            {
                eventData.DependencyAssetEvent?.Invoke(ne.DependencyAssetName);
            }
        }
    }
}
