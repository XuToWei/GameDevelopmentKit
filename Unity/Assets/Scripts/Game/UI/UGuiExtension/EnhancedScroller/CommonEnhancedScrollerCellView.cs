using System;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class CommonEnhancedScrollerCellView : EnhancedScrollerCellView
    {
        private Action<int, GameObject> m_ItemRenderer;
        private int m_DataIndex;

        public void SetRenderer(Action<int, GameObject> itemRenderer)
        {
            m_ItemRenderer = itemRenderer;
        }

        public void SetDataIndex(int dataIndex)
        {
            m_DataIndex = dataIndex;
        }
        
        public override void RefreshCellView()
        {
            base.RefreshCellView();
            m_ItemRenderer?.Invoke(m_DataIndex, gameObject);
        }
    }
}
