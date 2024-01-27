using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract class AUGuiForm : UIFormLogic
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
        }
    }
}