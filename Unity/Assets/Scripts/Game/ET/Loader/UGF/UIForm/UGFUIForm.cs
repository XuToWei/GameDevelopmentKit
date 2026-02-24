using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET
{
    public abstract class UGFUIForm<T> : UGFUIForm where T : AETMonoUGFUIForm
    {
        [BsonIgnore]
        public T View { get; private set; }
        
        [BsonIgnore]
        internal override AETMonoUGFUIForm UGFMono
        {
            get => base.UGFMono;
            set
            {
                if(value == null)
                {
                    base.UGFMono = null;
                    this.View = null;
                }
                else
                {
                    base.UGFMono = value;
                    this.View = (T)base.UGFMono;
                }
            }
        }
    }

    [EnableMethod]
    public abstract class UGFUIForm : Entity
    {
        [BsonIgnore]
        private UIForm m_UIForm;
        [BsonIgnore]
        private CancellationTokenSourcePlus m_Cts;
        [BsonIgnore]
        internal virtual AETMonoUGFUIForm UGFMono { get; set; }
        [BsonIgnore]
        public Transform CachedTransform { get; internal set; }
        [BsonIgnore]
        public bool Available => this.m_UIForm != null && this.m_UIForm.Logic.Available;
        [BsonIgnore]
        public bool Visible
        {
            get
            {
                return this.m_UIForm != null && this.m_UIForm.Logic.Visible;
            }
            set
            {
                if (this.m_UIForm == null)
                {
                    Log.Warning("UI form is not opened.");
                    return;
                }
                this.m_UIForm.Logic.Visible = value;
            }
        }

        public override void Dispose()
        {
            bool isDisposed = this.IsDisposed;
            if (!isDisposed)
            {
                if (this.m_Cts != null)
                {
                    this.m_Cts.Cancel();
                    ObjectPool.Instance.Recycle(this.m_Cts);
                    this.m_Cts = null;
                }
                if (this.Available)
                {
                    GameEntry.UI.CloseUIForm(this.m_UIForm);
                    this.m_UIForm = null;
                }
            }
            base.Dispose();
        }

        public async UniTask OpenUIFormAsync(int uiFormTypeId)
        {
            if(this.m_Cts == null)
            {
                this.m_Cts = ObjectPool.Instance.Fetch<CancellationTokenSourcePlus>();
            }
            this.m_UIForm = await GameEntry.UI.OpenUIFormAsync(uiFormTypeId, ETMonoUGFUIFormData.Create(this), cancellationToken: this.m_Cts.MallocToken());
            this.m_Cts.FreeToken();
            if(this.m_UIForm == null)
            {
                throw new Exception($"UGFUIForm OpenUIFormAsync failed! uiFormTypeId:'{uiFormTypeId}'.");
            }
        }

        public void OpenAllUIWidgets()
        {
            this.UGFMono.OpenAllUIWidgets();
        }

        public void RefocusUIForm()
        {
            GameEntry.UI.RefocusUIForm(this.m_UIForm);
        }

        public void SetUIFormInstanceLocked(bool locked)
        {
            GameEntry.UI.SetUIFormInstanceLocked(this.m_UIForm, locked);
        }

        public void SetUIFormInstancePriority(int priority)
        {
            GameEntry.UI.SetUIFormInstancePriority(this.m_UIForm, priority);
        }

        public async UniTask<T> LoadChildUIWidgetAsync<T>(int uiEntityTypeId) where T : UGFUIWidget, IAwake
        {
            var ugfEntity = this.AddChild<CommonUGFEntity>(true);
            await ugfEntity.ShowUIEntityAsync(uiEntityTypeId);
            var monoUIWidget = ugfEntity.CachedTransform.GetComponent<AETMonoUGFUIWidget>();
            if (monoUIWidget == null)
            {
                ugfEntity.Dispose();
                throw new Exception($"LoadChildUIWidgetAsync failed! not found AETMonoUGFUIWidget! uiEntityTypeId:'{uiEntityTypeId}'.");
            }
            T uiWidget = this.AddChildUIWidget<T>(monoUIWidget, true);
            uiWidget.AddChild(ugfEntity);
            return uiWidget;
        }

        public async UniTask<T> LoadComponentUIWidgetAsync<T>(int uiEntityTypeId) where T : UGFUIWidget, IAwake, new()
        {
            var ugfEntity = this.AddChild<CommonUGFEntity>(true);
            await ugfEntity.ShowUIEntityAsync(uiEntityTypeId);
            var monoUIWidget = ugfEntity.CachedTransform.GetComponent<AETMonoUGFUIWidget>();
            if (monoUIWidget == null)
            {
                ugfEntity.Dispose();
                throw new Exception($"LoadComponentUIWidgetAsync failed! not found AETMonoUGFUIWidget! uiEntityTypeId:'{uiEntityTypeId}'.");
            }
            return this.AddComponentUIWidget<T>(monoUIWidget, true);
        }

        public UGFUIWidget AddChildUIWidget(AETMonoUGFUIWidget etMonoWidget, bool isFromPool = false)
        {
            Type type = UGFSystemSingleton.Instance.GetWidgetType(etMonoWidget.GetType());
            UGFUIWidget widgetEntity = (UGFUIWidget)this.AddChild(type, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidget<T>(AETMonoUGFUIWidget etMonoWidget, bool isFromPool = false) where T : UGFUIWidget, IAwake
        {
            T widgetEntity = this.AddChild<T>(isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidget<T, A>(AETMonoUGFUIWidget etMonoWidget, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            T widgetEntity = this.AddChild<T, A>(a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidget<T, A, B>(AETMonoUGFUIWidget etMonoWidget, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            T widgetEntity = this.AddChild<T, A, B>(a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidget<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            T widgetEntity = this.AddChild<T, A, B, C>(a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidgetWithId<T>(AETMonoUGFUIWidget etMonoWidget, long id, bool isFromPool = false) where T : UGFUIWidget, IAwake
        {
            T widgetEntity = this.AddChildWithId<T>(id, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidgetWithId<T, A>(AETMonoUGFUIWidget etMonoWidget, long id, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            T widgetEntity = this.AddChildWithId<T, A>(id, a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidgetWithId<T, A, B>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            T widgetEntity = this.AddChildWithId<T, A, B>(id, a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidgetWithId<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            T widgetEntity = this.AddChildWithId<T, A, B, C>(id, a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T>(AETMonoUGFUIWidget etMonoWidget, bool isFromPool = false) where T : UGFUIWidget, IAwake, new()
        {
            T widgetEntity = this.AddComponent<T>(isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T, A>(AETMonoUGFUIWidget etMonoWidget, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>, new()
        {
            T widgetEntity = this.AddComponent<T, A>(a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T, A, B>(AETMonoUGFUIWidget etMonoWidget, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            T widgetEntity = this.AddComponent<T, A, B>(a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            T widgetEntity = this.AddComponent<T, A, B, C>(a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T>(AETMonoUGFUIWidget etMonoWidget, long id, bool isFromPool = false) where T : UGFUIWidget, IAwake, new()
        {
            T widgetEntity = this.AddComponentWithId<T>(id, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T, A>(AETMonoUGFUIWidget etMonoWidget, long id, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A>(id, a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T, A, B>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A, B>(id, a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A, B, C>(id, a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(widgetEntity));
            return widgetEntity;
        }
    }
}
