using System.Collections.Generic;
using GameFramework;

namespace Game
{
    public sealed class UIWidgetContainer : IReference
    {
        private readonly List<AUIWidget> m_UIWidgets = new List<AUIWidget>();
        public List<AUIWidget> UIWidgets => m_UIWidgets;

        public AUGuiForm Owner
        {
            get;
            private set;
        }

        public static UIWidgetContainer Create(AUGuiForm owner)
        {
            UIWidgetContainer uiWidgetContainer = ReferencePool.Acquire<UIWidgetContainer>();
            uiWidgetContainer.Owner = owner;
            return uiWidgetContainer;
        }

        public void Clear()
        {
            m_UIWidgets.Clear();
            Owner = null;
        }

        public void AddUIWidget(AUIWidget uiWidget, object userData = null)
        {
            if (uiWidget == null)
            {
                throw new GameFrameworkException("Can't add empty!");
            }
            if (m_UIWidgets.Contains(uiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't duplicate add UIWidget : '{0}'!", uiWidget.CachedTransform.name));
            }
            m_UIWidgets.Add(uiWidget);
            uiWidget.SetUIFormOwner(Owner.UIForm);
            uiWidget.OnInit(userData);
        }

        public bool HasUIWidget(AUIWidget uiWidget)
        {
            return m_UIWidgets.Contains(uiWidget);
        }

        public void RemoveUIWidget(AUIWidget uiWidget)
        {
            if (uiWidget == null)
            {
                throw new GameFrameworkException("Can't remove empty!");
            }
            if (!m_UIWidgets.Remove(uiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("UIWidget : '{0}' not in container.", uiWidget.CachedTransform.name));
            }
        }

        public void RemoveAllUIWidget()
        {
            if (m_UIWidgets.Count > 0)
            {
                m_UIWidgets.Clear();
            }
        }

        /// <summary>
        /// 打开UIWidget，不刷新Depth，一般在UIForm的OnOpen中调用
        /// </summary>
        /// <param name="uiWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void OpenUIWidget(AUIWidget uiWidget, object userData = null)
        {
            if (uiWidget == null)
            {
                throw new GameFrameworkException("Can't open empty!");
            }
            if (!m_UIWidgets.Contains(uiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UIWidget, UIWidget '{0}' not in the container '{1}'!", uiWidget.name, Owner.Name));
            }
            if (uiWidget.Available)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UIWidget, UIWidget '{0}' is already opened!", uiWidget.name));
            }
            uiWidget.OnOpen(userData);
        }

        /// <summary>
        /// 动态打开UIWidget，刷新Depth
        /// </summary>
        /// <param name="uiWidget"></param>
        /// <param name="userData"></param>
        public void DynamicOpenUIWidget(AUIWidget uiWidget, object userData = null)
        {
            OpenUIWidget(uiWidget, userData);
            uiWidget.OnDepthChanged(Owner.UIForm.UIGroup.Depth, Owner.UIForm.DepthInUIGroup);
        }

        public void CloseUIWidget(AUIWidget uiWidget, bool isShutdown, object userData = null)
        {
            if (uiWidget == null)
            {
                throw new GameFrameworkException("Can't open empty!");
            }
            if (!m_UIWidgets.Contains(uiWidget))
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't open UIWidget, UIWidget '{0}' not in the container '{1}'!", uiWidget.name, Owner.Name));
            }
            if (!uiWidget.Available)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can't close UIWidget, UIWidget '{0}' is not opened!", uiWidget.name));
            }
            uiWidget.OnClose(isShutdown, userData);
        }

        public void CloseAllUIWidgets(bool isShutdown, object userData = null)
        {
            if (m_UIWidgets.Count > 0)
            {
                foreach (var uiWidget in m_UIWidgets)
                {
                    if (uiWidget.Available)
                    {
                        uiWidget.OnClose(isShutdown, userData);
                    }
                }
            }
        }

        /// <summary>
        /// 界面回收。
        /// </summary>
        public void OnRecycle()
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                uiWidget.OnRecycle();
            }
        }

        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="isShutdown">是否是关闭界面管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void OnClose(bool isShutdown, object userData)
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnClose(isShutdown, userData);
                }
            }
        }

        /// <summary>
        /// 界面暂停。
        /// </summary>
        public void OnPause()
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnPause();
                }
            }
        }

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        public void OnResume()
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnResume();
                }
            }
        }

        /// <summary>
        /// 界面遮挡。
        /// </summary>
        public void OnCover()
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnCover();
                }
            }
        }

        /// <summary>
        /// 界面遮挡恢复。
        /// </summary>
        public void OnReveal()
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnReveal();
                }
            }
        }

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void OnRefocus(object userData)
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnRefocus(userData);
                }
            }
        }

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnUpdate(elapseSeconds, realElapseSeconds);
                }
            }
        }

        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            foreach (var uiWidget in m_UIWidgets)
            {
                if (uiWidget.Available)
                {
                    uiWidget.OnDepthChanged(uiGroupDepth, depthInUIGroup);
                }
            }
        }
    }
}
