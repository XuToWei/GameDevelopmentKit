using System;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace ET
{
    [RequireComponent(typeof(EnhancedScroller))]
    public class ETEnhancedScrollerDelegate : MonoBehaviour, IEnhancedScrollerDelegate
    {
        private EnhancedScroller m_EnhancedScroller;

        private Func<EnhancedScroller, int> m_GetNumberOfCellsAction;
        private Func<EnhancedScroller, int, float> m_GetCellViewSizeAction;
        private Func<EnhancedScroller, int, int, EnhancedScrollerCellView> m_GetCellViewAction;

        private void Awake()
        {
            this.m_EnhancedScroller = GetComponent<EnhancedScroller>();
            this.m_EnhancedScroller.Delegate = this;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return m_GetNumberOfCellsAction(scroller);
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return m_GetCellViewSizeAction(scroller, dataIndex);
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            return m_GetCellViewAction(scroller, dataIndex, cellIndex);
        }
    }
}
