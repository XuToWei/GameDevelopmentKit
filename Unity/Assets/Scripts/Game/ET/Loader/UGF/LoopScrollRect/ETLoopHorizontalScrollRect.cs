using Game;
using UnityEngine;

namespace ET
{
    /// <summary>
    /// ET 版横向循环列表：在 ExLoopHorizontalScrollRect 基础上，
    /// 当 item 是 AETMonoUGFUIWidget 时，取出时自动添加到所属 UIForm/父 UIWidget 并打开，回收时关闭并移除。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/ET Loop Horizontal Scroll Rect")]
    public class ETLoopHorizontalScrollRect : ExLoopHorizontalScrollRect
    {
        private AETMonoUGFUIWidget m_ParentUIWidget;
        private AETMonoUGFUIForm m_ParentUIForm;

        protected override void Awake()
        {
            base.Awake();
            // 缓存所属 UIForm / 父 UIWidget。owner 从循环列表自身向上查找（item 是其子节点，不会取到自己）。
            m_ParentUIWidget = GetComponentInParent<AETMonoUGFUIWidget>(true);
            if (m_ParentUIWidget == null)
            {
                m_ParentUIForm = GetComponentInParent<AETMonoUGFUIForm>(true);
            }
        }

        public override GameObject GetObject(int index)
        {
            GameObject go = base.GetObject(index);
            AETMonoUGFUIWidget uiWidget = go.GetComponent<AETMonoUGFUIWidget>();
            if (uiWidget != null && !uiWidget.Initialized)
            {
                if (m_ParentUIWidget != null)
                {
                    m_ParentUIWidget.UGFUIWidget.AddChildUIWidget(uiWidget, true);
                    uiWidget.TryDynamicOpen();
                }
                else if (m_ParentUIForm != null)
                {
                    m_ParentUIForm.UGFUIForm.AddChildUIWidget(uiWidget);
                    uiWidget.TryDynamicOpen();
                }
            }
            return go;
        }

        public override void ReturnObject(Transform trans)
        {
            AETMonoUGFUIWidget uiWidget = trans.GetComponent<AETMonoUGFUIWidget>();
            if (uiWidget != null && uiWidget.UGFUIWidget != null)
            {
                uiWidget.UGFUIWidget.TryClose();
            }
            base.ReturnObject(trans);
        }
    }
}
