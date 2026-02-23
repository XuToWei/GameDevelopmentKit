using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract class AExUIWidget : AUIWidget
    {
        private AExUIWidget m_ParentUIWidget;
        private AExUIForm m_UIForm;
        private UIWidgetContainer m_UIWidgetContainer;
        private EventContainer m_EventContainer;
        private EntityContainer m_EntityContainer;
        private ResourceContainer m_ResourceContainer;

        /// <summary>
        /// 父UIWidget
        /// </summary>
        public AExUIWidget ParentUIWidget => m_ParentUIWidget;

        /// <summary>
        /// 父UIForm
        /// </summary>
        public AExUIForm UIForm => m_UIForm;

        /// <summary>
        /// 打开所有UIWidget
        /// </summary>
        /// <param name="userData">userData</param>
        public void OpenAllUIWidgets(object userData = null)
        {
            if (m_UIWidgetContainer == null)
                return;
            UGFList<AUIWidget> uiWidgets = UGFList<AUIWidget>.Create();
            m_UIWidgetContainer.GetAllUIWidgets(uiWidgets);
            foreach (AUIWidget uiWidget in uiWidgets)
            {
                if (!uiWidget.Available)
                {
                    m_UIWidgetContainer.OpenUIWidget(uiWidget, userData);
                }
            }
            uiWidgets.Dispose();
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="userData">userData</param>
        public void Open(object userData = null)
        {
            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.OpenUIWidget(this, userData);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.OpenUIWidget(this, userData);
                return;
            }

            throw new GameFrameworkException("UI widget is invalid.");
        }

        /// <summary>
        /// 尝试打开
        /// </summary>
        /// <param name="userData">userData</param>
        public void TryOpen(object userData = null)
        {
            if (Available)
                return;

            if(m_ParentUIWidget != null)
            {
                m_ParentUIWidget.OpenUIWidget(this, userData);
                return;
            }

            if (m_UIForm != null)
            {
                m_UIForm.OpenUIWidget(this, userData);
                return;
            }
        }

        internal override void SetUIFormOwner(AUIForm uiForm)
        {
            base.SetUIFormOwner(uiForm);
            m_UIForm = uiForm as AExUIForm;
        }

        private void ClearContainer()
        {
            if (m_EventContainer != null)
            {
                ReferencePool.Release(m_EventContainer);
                m_EventContainer = null;
            }
            if (m_EntityContainer != null)
            {
                ReferencePool.Release(m_EntityContainer);
                m_EntityContainer = null;
            }
            if (m_UIWidgetContainer != null)
            {
                ReferencePool.Release(m_UIWidgetContainer);
                m_UIWidgetContainer = null;
            }
            if (m_ResourceContainer != null)
            {
                ReferencePool.Release(m_ResourceContainer);
                m_ResourceContainer = null;
            }
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);

            UGFList<AExUIWidget> uiWidgets = new UGFList<AExUIWidget>();
            GetComponentsInChildren(true, uiWidgets);
            foreach (AExUIWidget uiWidget in uiWidgets)
            {
                if (uiWidget == this || uiWidget.UIFormOwner != null || uiWidget.GetComponentInParent<AExUIWidget>(true) != this)
                    continue;
                AddUIWidget(uiWidget, userData);
            }
            uiWidgets.Dispose();
        }

        protected internal override void OnRecycle()
        {
            base.OnRecycle();
            m_UIWidgetContainer?.OnRecycle();
        }

        protected virtual void OnDestroy()
        {
            RemoveAllUIWidget();
            ClearContainer();
        }

        protected internal override void OnClose(bool isShutdown, object userData)
        {
            m_UIWidgetContainer?.OnClose(isShutdown, userData);
            HideAllEntity(isShutdown);
            UnsubscribeAll(isShutdown);
            UnloadAllAssets(isShutdown);
            CloseAllUIWidgets(isShutdown, userData);
            if (isShutdown)
            {
                RemoveAllUIWidget();
                ClearContainer();
            }
            base.OnClose(isShutdown, userData);
        }
        
        protected internal override void OnPause()
        {
            base.OnPause();
            m_UIWidgetContainer?.OnPause();
        }
        
        protected internal override void OnResume()
        {
            base.OnResume();
            m_UIWidgetContainer?.OnResume();
        }

        protected internal override void OnCover()
        {
            base.OnCover();
            m_UIWidgetContainer?.OnCover();
        }

        protected internal override void OnReveal()
        {
            base.OnReveal();
            m_UIWidgetContainer?.OnReveal();
        }

        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            m_UIWidgetContainer?.OnRefocus(userData);
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            m_UIWidgetContainer?.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            m_UIWidgetContainer?.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public void AddUIWidget(AExUIWidget aExUIWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                m_UIWidgetContainer = UIWidgetContainer.Create(UIFormOwner);
            }
            aExUIWidget.m_ParentUIWidget = this;
            m_UIWidgetContainer.AddUIWidget(aExUIWidget, userData);
        }

        public bool HasUIWidget(AExUIWidget uiWidget)
        {
            if(m_UIWidgetContainer == null)
            {
                return false;
            }
            return m_UIWidgetContainer.HasUIWidget(uiWidget);
        }

        public void RemoveUIWidget(AExUIWidget aExUIWidget)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.RemoveUIWidget(aExUIWidget);
            aExUIWidget.m_ParentUIWidget = null;
        }

        public void RemoveAllUIWidget()
        {
            if (m_UIWidgetContainer == null)
                return;
            using UGFList<AExUIWidget> aExUIWidgets = UGFList<AExUIWidget>.Create();
            foreach (AUIWidget uiWidget in m_UIWidgetContainer.UIWidgets)
            {
                AExUIWidget aExUIWidget = (AExUIWidget)uiWidget;
                aExUIWidgets.Add(aExUIWidget);
            }
            m_UIWidgetContainer.RemoveAllUIWidget();
            foreach (AExUIWidget aExUIWidget in aExUIWidgets)
            {
                aExUIWidget.m_ParentUIWidget = null;
            }
        }

        /// <summary>
        /// 打开UIWidget，不刷新Depth，一般在UIForm的OnOpen中调用
        /// </summary>
        /// <param name="aExUIWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void OpenUIWidget(AExUIWidget aExUIWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.OpenUIWidget(aExUIWidget, userData);
        }

        /// <summary>
        /// 动态打开UIWidget，刷新Depth
        /// </summary>
        /// <param name="aExUIWidget"></param>
        /// <param name="userData"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void DynamicOpenUIWidget(AExUIWidget aExUIWidget, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.DynamicOpenUIWidget(aExUIWidget, userData);
        }

        public void CloseUIWidget(AExUIWidget uiWidget, bool isShutdown = false, object userData = null)
        {
            if (m_UIWidgetContainer == null)
            {
                throw new GameFrameworkException("Container is empty!");
            }
            m_UIWidgetContainer.CloseUIWidget(uiWidget, isShutdown, userData);
        }

        public void CloseAllUIWidgets(bool isShutdown = false, object userData = null)
        {
            if (m_UIWidgetContainer == null)
                return;
            m_UIWidgetContainer.CloseAllUIWidgets(isShutdown, userData);
        }

        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null)
            {
                m_EventContainer = EventContainer.Create(this);
            }
            m_EventContainer.Subscribe(id, handler);
        }

        public void Unsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null)
                return;
            m_EventContainer.Unsubscribe(id, handler);
        }

        public void TryUnsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventContainer == null)
                return;
            m_EventContainer.TryUnsubscribe(id, handler);
        }

        public void UnsubscribeAll()
        {
            UnsubscribeAll(false);
        }

        public void UnsubscribeAll(bool isShutdown)
        {
            if (m_EventContainer == null)
                return;
            m_EventContainer.UnsubscribeAll(isShutdown);
        }

        public void TryUnsubscribeAll()
        {
            TryUnsubscribeAll(false);
        }

        public void TryUnsubscribeAll(bool isShutdown)
        {
            if (m_EventContainer == null)
                return;
            m_EventContainer.TryUnsubscribeAll(isShutdown);
        }

        public int? ShowEntity<T>(int entityTypeId, Action<Entity> onShowSuccess, Action onShowFailure = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity<T>(entityTypeId, onShowSuccess, onShowFailure);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, Action<Entity> onShowSuccess, Action onShowFailure = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity(entityTypeId, logicType, onShowSuccess, onShowFailure);
        }

        public int? ShowEntity<T>(int entityTypeId, object userData = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity<T>(entityTypeId, userData);
        }

        public int? ShowEntity(int entityTypeId, Type logicType, object userData = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntity(entityTypeId, logicType, userData);
        }

        public UniTask<Entity> ShowEntityAsync<T>(int entityTypeId, object userData = null) where T : EntityLogic
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntityAsync(entityTypeId, typeof(T), userData);
        }

        public UniTask<Entity> ShowEntityAsync(int entityTypeId, Type logicType, object userData = null)
        {
            if (m_EntityContainer == null)
            {
                m_EntityContainer = EntityContainer.Create(this);
            }
            return m_EntityContainer.ShowEntityAsync(entityTypeId, logicType, userData);
        }

        public void HideAllEntity()
        {
            HideAllEntity(false);
        }

        public void HideAllEntity(bool isShutdown)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.HideAllEntity(isShutdown);
        }

        public void TryHideAllEntity()
        {
            TryHideAllEntity(false);
        }

        public void TryHideAllEntity(bool isShutdown)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.TryHideAllEntity(isShutdown);
        }

        public void HideEntity(int serialId)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.HideEntity(serialId);
        }

        public void HideEntity(Entity entity)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.HideEntity(entity);
        }

        public void TryHideEntity(int serialId)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.TryHideEntity(serialId);
        }

        public void TryHideEntity(Entity entity)
        {
            if (m_EntityContainer == null)
                return;
            m_EntityContainer.TryHideEntity(entity);
        }

        public void LoadAsset<T>(string assetName, Action<T> onLoadSuccess, Action onLoadFailure = null, int priority = 0,
            Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            if (m_ResourceContainer == null)
            {
                m_ResourceContainer = ResourceContainer.Create(this);
            }
            m_ResourceContainer.LoadAsset(assetName, onLoadSuccess, onLoadFailure, priority, updateEvent, dependencyAssetEvent);
        }

        public UniTask<T> LoadAssetAsync<T>(string assetName, int priority = 0,
            Action<float> updateEvent = null, Action<string> dependencyAssetEvent = null) where T : UnityEngine.Object
        {
            if (m_ResourceContainer == null)
            {
                m_ResourceContainer = ResourceContainer.Create(this);
            }
            return m_ResourceContainer.LoadAssetAsync<T>(assetName, priority, updateEvent, dependencyAssetEvent);
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (m_ResourceContainer == null)
                return;
            m_ResourceContainer.UnloadAsset(asset);
        }

        public void UnloadAllAssets()
        {
            if (m_ResourceContainer == null)
                return;
            m_ResourceContainer.UnloadAllAssets();
        }

        public void UnloadAllAssets(bool isShutdown)
        {
            if (m_ResourceContainer == null)
                return;
            m_ResourceContainer.UnloadAllAssets(isShutdown);
        }
    }
}
