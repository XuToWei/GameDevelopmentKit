using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game
{
    public sealed class EntityContainer : IReference
    {
        private sealed class EntityLoaderEventArgs : GameEventArgs
        {
            private static readonly int s_EventId = typeof(ShowEntitySuccessEventArgs).GetHashCode();
            public override int Id => s_EventId;

            public Action<Entity> OnSuccessCallback { get; private set; }
            public Action OnFailureCallback { get; private set; }

            public static EntityLoaderEventArgs Create(Action<Entity> onSuccessCallback, Action onFailureCallback)
            {
                EntityLoaderEventArgs eventArgs = ReferencePool.Acquire<EntityLoaderEventArgs>();
                eventArgs.OnSuccessCallback = onSuccessCallback;
                eventArgs.OnFailureCallback = onFailureCallback;
                return eventArgs;
            }

            public override void Clear()
            {
                OnSuccessCallback = null;
                OnFailureCallback = null;
            }
        }

        private readonly HashSet<int> m_EntityIds = new HashSet<int>();
        private CancellationTokenSource m_CancellationTokenSource;

        public object Owner
        {
            get;
            private set;
        }

        public static EntityContainer Create(object owner)
        {
            EntityContainer entityContainer = ReferencePool.Acquire<EntityContainer>();
            entityContainer.Owner = owner;
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, entityContainer.OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, entityContainer.OnShowEntityFail);
            return entityContainer;
        }

        public void Clear()
        {
            m_EntityIds.Clear();
            m_CancellationTokenSource = null;
            Owner = null;
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFail);
        }

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = e as ShowEntitySuccessEventArgs;
            if (ne == null)
            {
                return;
            }
            EntityLoaderEventArgs eventArgs = ne.UserData as EntityLoaderEventArgs;
            if (eventArgs == null)
            {
                return;
            }
            eventArgs.OnSuccessCallback?.Invoke(ne.Entity);
            ReferencePool.Release(eventArgs);
        }

        private void OnShowEntityFail(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = e as ShowEntityFailureEventArgs;
            if (ne == null)
            {
                return;
            }
            EntityLoaderEventArgs eventArgs = ne.UserData as EntityLoaderEventArgs;
            if (eventArgs == null)
            {
                return;
            }
            ReferencePool.Release(eventArgs);
        }

        public int? ShowEntity<T>(int entityTypeId, Action<Entity> onShowSuccess) where T : EntityLogic
        {
            return ShowEntity<T>(entityTypeId, onShowSuccess, null);
        }

        public int? ShowEntity<T>(int entityTypeId, Action<Entity> onShowSuccess, Action onShowFailure) where T : EntityLogic
        {
            return ShowEntity(entityTypeId, typeof(T), onShowSuccess, onShowFailure);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, Action<Entity> onShowSuccess)
        {
            return ShowEntity(entityTypeId, logicType, onShowSuccess, null);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, Action<Entity> onShowSuccess, Action onShowFailure)
        {
            int? serialId = GameEntry.Entity.ShowEntity(entityTypeId, logicType, EntityLoaderEventArgs.Create(onShowSuccess, onShowFailure));
            if (serialId.HasValue)
            {
                m_EntityIds.Add(serialId.Value);
            }
            return serialId;
        }

        public int? ShowEntity<T>(int entityTypeId) where T : EntityLogic
        {
            return ShowEntity<T>(entityTypeId, null);
        }

        public int? ShowEntity<T>(int entityTypeId, object userData) where T : EntityLogic
        {
            return ShowEntity(entityTypeId, typeof(T), userData);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, object userData)
        {
            int? serialId = GameEntry.Entity.ShowEntity(entityTypeId, logicType, userData);
            if (serialId.HasValue)
            {
                m_EntityIds.Add(serialId.Value);
            }
            return serialId;
        }

        public UniTask<Entity> ShowEntityAsync<T>(int entityTypeId) where T : EntityLogic
        {
            return ShowEntityAsync<T>(entityTypeId, null);
        }

        public UniTask<Entity> ShowEntityAsync<T>(int entityTypeId, object userData) where T : EntityLogic
        {
            return ShowEntityAsync(entityTypeId, typeof(T), userData);
        }

        public UniTask<Entity> ShowEntityAsync(int entityTypeId, Type logicType)
        {
            return ShowEntityAsync(entityTypeId, logicType, null);
        }

        public async UniTask<Entity> ShowEntityAsync(int entityTypeId, Type logicType, object userData)
        {
            if (m_CancellationTokenSource == null)
            {
                m_CancellationTokenSource = new CancellationTokenSource();
            }
            Entity entity = await GameEntry.Entity.ShowEntityAsync(entityTypeId, logicType, userData, m_CancellationTokenSource.Token);
            m_EntityIds.Add(entity.Id);
            return entity;
        }

        public void HideAllEntity()
        {
            HideAllEntity(false);
        }

        public void HideAllEntity(bool isShutdown)
        {
            if (!isShutdown)
            {
                foreach (int serialId in m_EntityIds)
                {
                    GameEntry.Entity.HideEntity(serialId);
                }
            }

            m_EntityIds.Clear();
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource = null;
            }
        }

        public void TryHideAllEntity()
        {
            TryHideAllEntity(false);
        }

        public void TryHideAllEntity(bool isShutdown)
        {
            if (!isShutdown)
            {
                foreach (int serialId in m_EntityIds)
                {
                    GameEntry.Entity.TryHideEntity(serialId);
                }
            }

            m_EntityIds.Clear();
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource = null;
            }
        }

        public int[] GetEntityIds()
        {
            int[] entityIds = new int[m_EntityIds.Count];
            int index = 0;
            foreach (int entityId in m_EntityIds)
            {
                entityIds[index++] = entityId;
            }
            return entityIds;
        }

        public void GetEntityIds(List<int> result)
        {
            if (result == null)
            {
                throw new GameFrameworkException("Result list is invalid.");
            }
            result.Clear();
            foreach (int entityId in m_EntityIds)
            {
                result.Add(entityId);
            }
        }

        public void HideAllLoadingEntity()
        {
            HideAllEntity(false);
        }

        public void HideAllLoadingEntity(bool isShutdown)
        {
            if (!isShutdown)
            {
                using UGFList<int> entityIds = UGFList<int>.Create();
                GetEntityIds(entityIds);
                foreach (int entityId in entityIds)
                {
                    if (GameEntry.Entity.IsLoadingEntity(entityId))
                    {
                        HideEntity(entityId);
                    }
                }
            }
        }

        public void HideEntity(int serialId)
        {
            if (!m_EntityIds.Contains(serialId))
            {
                throw new GameFrameworkException(Utility.Text.Format("Entity serialId : '{0}' not in container.", serialId));
            }
            m_EntityIds.Remove(serialId);
            GameEntry.Entity.HideEntity(serialId);
        }

        public void HideEntity(Entity entity)
        {
            HideEntity(entity.Id);
        }

        public void TryHideEntity(int serialId)
        {
            if (m_EntityIds.Contains(serialId))
            {
                m_EntityIds.Remove(serialId);
                GameEntry.Entity.TryHideEntity(serialId);
            }
        }

        public void TryHideEntity(Entity entity)
        {
            TryHideEntity(entity.Id);
        }
    }
}