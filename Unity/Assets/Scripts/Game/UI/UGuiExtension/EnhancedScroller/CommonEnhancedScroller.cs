using System;
using EnhancedUI.EnhancedScroller;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnhancedScroller))]
    public class CommonEnhancedScroller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField]
        private CommonEnhancedScrollerCellView m_CellViewPrefab;
        
        private int m_NumItems;
        
        private float m_CellViewSize;
        
        public Action<int, GameObject> itemRenderer { set; private get; }
        
        [ShowInInspector, DisableInEditorMode]
        public int numItems
        {
            set
            {
                m_NumItems = value;
                scroller.ReloadData();
            }
            get => m_NumItems;
        }

        public EnhancedScroller scroller { get; private set; }

        private void Awake()
        {
            scroller = GetComponent<EnhancedScroller>();
            scroller.Delegate = this;
            m_CellViewSize = m_CellViewPrefab.GetComponent<RectTransform>().rect.height;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return m_NumItems;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return m_CellViewSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            CommonEnhancedScrollerCellView cellView = scroller.GetCellView(m_CellViewPrefab) as CommonEnhancedScrollerCellView;
            cellView.name = Utility.Text.Format("Cell {0}", dataIndex);
            cellView.SetRenderer(itemRenderer);
            cellView.SetDataIndex(dataIndex);
            itemRenderer?.Invoke(dataIndex, cellView.gameObject);
            return cellView;
        }
    }
}
