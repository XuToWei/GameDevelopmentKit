using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract class AExEntity : AEntity
    {
        private EventContainer m_EventContainer;
        private EntityContainer m_EntityContainer;
        private ResourceContainer m_ResourceContainer;

        private void ClearEntity()
        {
            if (m_EventContainer != null)
            {
                ReferencePool.Release(m_EventContainer);
                m_EventContainer = null;
            }
            if (m_EntityContainer != null)
            {
                ReferencePool.Release(m_EntityContainer);
                m_EntityContainer = null;
            }
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            ClearEntity();
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            HideAllEntity(isShutdown);
            UnsubscribeAll(isShutdown);
            if (isShutdown)
            {
                ClearEntity();
            }
            base.OnHide(isShutdown, userData);
        }

        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null)
            {
                m_EventContainer = EventContainer.Create(this);
            }
            m_EventContainer.Subscribe(id, handler);
        }

        public void Unsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null)
                return;
            m_EventContainer.Unsubscribe(id, handler);
        }

        public void UnsubscribeAll(bool isShutdown)
        {
            if (m_EventContainer == null)
                return;
            m_EventContainer.UnsubscribeAll(isShutdown);
        }

        public int? ShowEntity<T>(int entityTypeId, Action<Entity> onShowSuccess = null, Action onShowFailure = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity<T>(entityTypeId, onShowSuccess, onShowFailure);
        }

        public int? ShowEntity<T>(int entityTypeId, object userData = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity<T>(entityTypeId, userData);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, object userData = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity(entityTypeId, logicType, userData);
        }

        public UniTask<Entity> ShowEntityAsync<T>(int entityTypeId, object userData = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntityAsync(entityTypeId, typeof(T), userData);
        }

        public UniTask<Entity> ShowEntityAsync(int entityTypeId, Type logicType, object userData = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntityAsync(entityTypeId, logicType, userData);
        }

        public void HideAllEntity(bool isShutdown)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.HideAllEntity(isShutdown);
        }

        public void HideEntity(int serialId)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.HideEntity(serialId);
        }

        public void HideEntity(Entity entity)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.HideEntity(entity);
        }

        public void LoadAsset<T>(string assetName, Action<T> onLoadSuccess, Action onLoadFailure = null, int priority = 0,
            Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            if (m_ResourceContainer == null)
            {
                m_ResourceContainer = ResourceContainer.Create(this);
            }
            m_ResourceContainer.LoadAsset(assetName, onLoadSuccess, onLoadFailure, priority, updateEvent, dependencyAssetEvent);
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName, int priority = 0,
            Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            if (m_ResourceContainer == null)
            {
                m_ResourceContainer = ResourceContainer.Create(this);
            }
            return await m_ResourceContainer.LoadAssetAsync<T>(assetName, priority, updateEvent, dependencyAssetEvent);
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (m_ResourceContainer == null)
                return;
            m_ResourceContainer.UnloadAsset(asset);
        }

        public void UnloadAllAssets()
        {
            if (m_ResourceContainer == null)
                return;
            m_ResourceContainer.UnloadAllAssets();
        }
    }
}