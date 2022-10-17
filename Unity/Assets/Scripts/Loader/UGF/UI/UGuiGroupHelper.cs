//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace UGF
{
    /// <summary>
    /// uGUI 界面组辅助器。
    /// </summary>
    public class UGuiGroupHelper : UIGroupHelperBase
    {
        public const int DepthFactor = 10000;

        private int m_Depth = 0;
        private Canvas m_CachedCanvas = null;

        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        public override void SetDepth(int depth)
        {
            m_Depth = depth;
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor * depth;
        }

        private void Awake()
        {
            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor * m_Depth;

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
           
            float screenRatio = GameEntry.Screen.UIHeight * 1f / GameEntry.Screen.UIWidth;
            if (screenRatio > GameEntry.Screen.StandardVerticalRatio)
            {
                float offset = GameEntry.Screen.UIHeight - 1f * GameEntry.Screen.UIWidth /
                        GameEntry.Screen.StandardWidth * GameEntry.Screen.StandardHeight;
                rectTransform.sizeDelta = new Vector2(0, -offset);
            }
            else
            {
                float offset = GameEntry.Screen.UIWidth - 1f * GameEntry.Screen.UIHeight /
                        GameEntry.Screen.StandardHeight * GameEntry.Screen.StandardWidth;
                rectTransform.sizeDelta = new Vector2(-offset, 0);
            }
        }
    }
}
