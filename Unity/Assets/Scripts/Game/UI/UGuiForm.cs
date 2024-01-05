using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract class UGuiForm : UIFormLogic
    {
        public const int DepthFactor = 100;

        private Canvas m_CachedCanvas = null;
        private readonly List<ParticleSystemRenderer> m_CachedParticleSystemRenderersContainer = new List<ParticleSystemRenderer>();
        private readonly List<Canvas> m_CachedCanvasContainer = new List<Canvas>();
        private EventSubscriber m_EventSubscriber;
        private EntityLoader m_EntityLoader;

        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return m_CachedCanvas.sortingOrder;
            }
        }

        public virtual void Close()
        {
            GameEntry.UI.CloseUIForm(this.UIForm);
        }

        public virtual void PlayUISound(int uiSoundId)
        {
            GameEntry.Sound.PlayUISound(uiSoundId);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected internal override void OnInit(object userData)
#endif
        {
            base.OnInit(userData);

            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            OriginalDepth = m_CachedCanvas.sortingOrder;

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnRecycle()
#else
        protected internal override void OnRecycle()
#endif
        {
            base.OnRecycle();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected internal override void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(bool isShutdown, object userData)
#else
        protected internal override void OnClose(bool isShutdown, object userData)
#endif
        {
            if (m_EventSubscriber != null)
            {
                ReferencePool.Release(m_EventSubscriber);
            }
            if (m_EntityLoader != null)
            {
                ReferencePool.Release(m_EntityLoader);
            }
            base.OnClose(isShutdown, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnPause()
#else
        protected internal override void OnPause()
#endif
        {
            base.OnPause();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnResume()
#else
        protected internal override void OnResume()
#endif
        {
            base.OnResume();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnCover()
#else
        protected internal override void OnCover()
#endif
        {
            base.OnCover();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnReveal()
#else
        protected internal override void OnReveal()
#endif
        {
            base.OnReveal();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnRefocus(object userData)
#else
        protected internal override void OnRefocus(object userData)
#endif
        {
            base.OnRefocus(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#else
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#endif
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
#else
        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
#endif
        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            GetComponentsInChildren(true, m_CachedCanvasContainer);
            for (int i = 0; i < m_CachedCanvasContainer.Count; i++)
            {
                m_CachedCanvasContainer[i].sortingOrder += deltaDepth;
            }
            m_CachedCanvasContainer.Clear();
            
            GetComponentsInChildren(true, m_CachedParticleSystemRenderersContainer);
            foreach (var t in m_CachedParticleSystemRenderersContainer)
            {
                t.sortingOrder += deltaDepth;
            }
            m_CachedParticleSystemRenderersContainer.Clear();
        }

        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventSubscriber == null)
            {
                m_EventSubscriber = EventSubscriber.Create(this);
            }
            m_EventSubscriber.Subscribe(id, handler);
        }

        public void Unsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventSubscriber == null)
                return;
            m_EventSubscriber.Unsubscribe(id, handler);
        }

        public void UnsubscribeAll()
        {
            if (m_EventSubscriber == null)
                return;
            m_EventSubscriber.UnsubscribeAll();
        }

        public int? ShowEntity(int entityTypeId, Action<Entity> onShowSuccess, Action onShowFailure = default)
        {
            if (m_EntityLoader == null)
            {
                m_EntityLoader = EntityLoader.Create(this);
            }
            return m_EntityLoader.ShowEntity(entityTypeId, onShowSuccess, onShowFailure);
        }

        public int? ShowEntity<T>(int entityTypeId, object userData) where T : EntityLogic
        {
            if (m_EntityLoader == null)
            {
                m_EntityLoader = EntityLoader.Create(this);
            }
            return m_EntityLoader.ShowEntity<T>(entityTypeId, userData);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, object userData)
        { 
            if (m_EntityLoader == null)
            {
                m_EntityLoader = EntityLoader.Create(this);
            }
            return m_EntityLoader.ShowEntity(entityTypeId, logicType, userData);
        }
        
        public UniTask<Entity> ShowEntityAsync(int entityTypeId, object userData)
        {
            if (m_EntityLoader == null)
            {
                m_EntityLoader = EntityLoader.Create(this);
            }
            return m_EntityLoader.ShowEntityAsync(entityTypeId, typeof(ItemEntity), userData);
        }

        public UniTask<Entity> ShowEntityAsync<T>(int entityTypeId, object userData) where T : EntityLogic
        {
            if (m_EntityLoader == null)
            {
                m_EntityLoader = EntityLoader.Create(this);
            }
            return m_EntityLoader.ShowEntityAsync(entityTypeId, typeof(T), userData);
        }

        public UniTask<Entity> ShowEntityAsync(int entityTypeId, Type logicType, object userData)
        {
            if (m_EntityLoader == null)
            {
                m_EntityLoader = EntityLoader.Create(this);
            }
            return m_EntityLoader.ShowEntityAsync(entityTypeId, logicType, userData);
        }
        
        public void HideAllEntity()
        {
            if(m_EntityLoader == null)
                return;
            m_EntityLoader.HideAllEntity();
        }

        public void HideEntity(int serialId)
        {
            if(m_EntityLoader == null)
                return;
            m_EntityLoader.HideEntity(serialId);
        }

        public void HideEntity(Entity entity)
        {
            if(m_EntityLoader == null)
                return;
            m_EntityLoader.HideEntity(entity);
        }
    }
}