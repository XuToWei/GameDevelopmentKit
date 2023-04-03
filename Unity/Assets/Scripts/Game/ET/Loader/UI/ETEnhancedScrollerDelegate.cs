using System;
using CodeBind;
using EnhancedUI.EnhancedScroller;
using GameFramework;
using UnityEngine;

namespace ET
{
    [CodeBindName("ETEnhancedScrollerDelegate")]
    [RequireComponent(typeof(EnhancedScroller))]
    public class ETEnhancedScrollerDelegate : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private ETEnhancedScrollerCellView m_EnhancedScrollerCellView;
        
        private EnhancedScroller m_EnhancedScroller;
        private float m_Size;
        
        private int m_NumberOfCells;
        private Action<EnhancedScroller, ETEnhancedScrollerCellView, int, int> m_GetCellViewAction;
        
        private void Awake()
        {
            this.m_NumberOfCells = 0;
            this.m_EnhancedScroller = GetComponent<EnhancedScroller>();
            this.m_EnhancedScroller.Delegate = this;
            if (this.m_EnhancedScrollerCellView == null)
            {
                throw new GameFrameworkException("CellView is empty!");
            }

            if (this.m_EnhancedScroller.scrollDirection == EnhancedScroller.ScrollDirectionEnum.Horizontal)
            {
                this.m_Size = this.m_EnhancedScrollerCellView.rectTransform.rect.width;
            }
            else if (this.m_EnhancedScroller.scrollDirection == EnhancedScroller.ScrollDirectionEnum.Vertical)
            {
                this.m_Size = this.m_EnhancedScrollerCellView.rectTransform.rect.height;
            }
        }

        /// <summary>
        /// 设置滑动列表
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="cellViewAction">cellView刷新的Action</param>
        public void Set(int count, Action<EnhancedScroller, ETEnhancedScrollerCellView, int, int> cellViewAction)
        {
            this.m_NumberOfCells = count;
            this.m_GetCellViewAction = cellViewAction;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return this.m_NumberOfCells;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return this.m_Size;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            ETEnhancedScrollerCellView cellView = (ETEnhancedScrollerCellView)scroller.GetCellView(m_EnhancedScrollerCellView);
#if UNITY_EDITOR
            cellView.name = Utility.Text.Format("Cell(Editor)-{0}", dataIndex);
#endif
            this.m_GetCellViewAction(scroller, cellView, dataIndex, cellIndex);
            return cellView;
        }
    }
}
