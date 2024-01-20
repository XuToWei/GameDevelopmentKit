using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract partial class UGuiForm : UIFormLogic
    {
        public const int DepthFactor = 100;

        private Canvas m_CachedCanvas = null;

        private readonly List<ParticleSystemRenderer> m_CachedParticleSystemRenderersContainer = new List<ParticleSystemRenderer>();

        private readonly List<Canvas> m_CachedCanvasContainer = new List<Canvas>();

        public int OriginalDepth { get; private set; }

        public int Depth => m_CachedCanvas.sortingOrder;

        public virtual void Close()
        {
            GameEntry.UI.CloseUIForm(this.UIForm);
        }

        public virtual void PlayUISound(int uiSoundId)
        {
            GameEntry.Sound.PlayUISound(uiSoundId);
        }

        protected override void OnInit(object userData)
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
            UIWidget[] uiWidgets = gameObject.GetComponentsInChildren<UIWidget>();
            if (uiWidgets != null && uiWidgets.Length > 0)
            {
                for (int i = 0; i < uiWidgets.Length; i++)
                {
                    AddUIWidget(uiWidgets[i]);
                }
            }
            m_UIWidgetContainer?.OnInit(userData);
        }

        private void OnDestroy()
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
            if (m_UIWidgetContainer != null)
            {
                ReferencePool.Release(m_UIWidgetContainer);
                m_UIWidgetContainer = null;
            }
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            m_UIWidgetContainer?.OnRecycle();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_UIWidgetContainer?.OnOpen(userData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_UIWidgetContainer?.OnClose(isShutdown, userData);
            base.OnClose(isShutdown, userData);
        }

        protected override void OnPause()
        {
            base.OnPause();
            m_UIWidgetContainer?.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            m_UIWidgetContainer?.OnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
            m_UIWidgetContainer?.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            m_UIWidgetContainer?.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            m_UIWidgetContainer?.OnRefocus(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            m_UIWidgetContainer?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
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
            m_UIWidgetContainer?.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }
    }
}