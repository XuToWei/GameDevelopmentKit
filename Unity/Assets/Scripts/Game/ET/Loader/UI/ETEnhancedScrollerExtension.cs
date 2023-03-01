using System;
using EnhancedUI.EnhancedScroller;

namespace ET
{
    public static class ETEnhancedScrollerExtension
    {
        /// <summary>
        /// 设置滑动列表
        /// </summary>
        /// <param name="enhancedScroller"></param>
        /// <param name="count">数量</param>
        /// <param name="cellViewAction">cellView刷新的Action</param>
        public static void Set(this EnhancedScroller enhancedScroller, int count, Action<EnhancedScroller, ETEnhancedScrollerCellView, int, int> cellViewAction)
        {
            ETEnhancedScrollerDelegate scrollerDelegate = enhancedScroller.GetComponent<ETEnhancedScrollerDelegate>();
            scrollerDelegate.Set(count, cellViewAction);
        }
    }
}
