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
            eventData.DependencyAssetEvent = dependencyAssetEvent;

            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            s_ShowEntityEventDataDict.Add(entityId, eventData);

            bool MoveNext(ref UniTaskCompletionSourceCore<Entity> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    if (entityComponent.HasEntity(entityId))
                    {
                        entityComponent.HideEntity(entityId);
                    }
                    return false;
                }
                if (entityComponent.IsLoadingEntity(entityId))
                {
                    return true;
                }
                Entity entity = entityComponent.GetEntity(entityId);
                if (entity == null)//这里是被其他接口关闭了
                {
                    core.TrySetException(new GameFrameworkException(Utility.Text.Format("Show entity task is failure, entity id '{0}', asset name '{1}', entity group name '{2}'.", entityId, entityAssetName, entityGroupName)));
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

        private sealed class ShowEntityEventData : IReference
        {
            public Action<float> UpdateEvent;
            public Action<string> DependencyAssetEvent;

            public void Clear()
            {
                UpdateEvent = null;
                DependencyAssetEvent = null;
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
