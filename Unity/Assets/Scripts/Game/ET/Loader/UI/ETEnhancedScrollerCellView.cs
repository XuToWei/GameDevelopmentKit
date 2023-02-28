using CodeBind;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace ET
{
    [RequireComponent(typeof(CSCodeBindMono))]
    public class ETEnhancedScrollerCellView : EnhancedScrollerCellView
    {
        private CSCodeBindMono m_CSCodeBindMono;
        private void Awake()
        {
            this.m_CSCodeBindMono = GetComponent<CSCodeBindMono>();
        }
        
        
    }
}
