using System;
using Cysharp.Threading.Tasks;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Game
{
    public partial class UGuiForm
    {
        private UIWidgetContainer m_UIWidgetContainer;
        private EventContainer m_EventContainer;
        private EntityContainer m_EntityContainer;

        public void AddUIWidget(UIWidget uiWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                m_UIWidgetContainer = UIWidgetContainer.Create(this);
            }
            m_UIWidgetContainer.AddUIWidget(uiWidget);
        }

        public void RemoveUIWidget(UIWidget uiWidget)
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.RemoveUIWidget(uiWidget);
        }

        public void RemoveAllUIWidget()
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.RemoveAllUIWidget();
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

        public void UnsubscribeAll()
        {
            if (m_EventContainer == null)
                return;
            m_EventContainer.UnsubscribeAll();
        }

        public int? ShowEntity(int entityTypeId, Action<Entity> onShowSuccess, Action onShowFailure = default)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity(entityTypeId, onShowSuccess, onShowFailure);
        }

        public int? ShowEntity<T>(int entityTypeId, object userData) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity<T>(entityTypeId, userData);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, object userData)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity(entityTypeId, logicType, userData);
        }

        public UniTask<Entity> ShowEntityAsync(int entityTypeId, object userData)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntityAsync(entityTypeId, typeof(ItemEntity), userData);
        }

        public UniTask<Entity> ShowEntityAsync<T>(int entityTypeId, object userData) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntityAsync(entityTypeId, typeof(T), userData);
        }

        public UniTask<Entity> ShowEntityAsync(int entityTypeId, Type logicType, object userData)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntityAsync(entityTypeId, logicType, userData);
        }

        public void HideAllEntity()
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.HideAllEntity();
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
    }
}