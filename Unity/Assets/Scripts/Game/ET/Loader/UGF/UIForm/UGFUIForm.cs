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
                base.UGFMono = value;
                View = (T)base.UGFMono;
            }
        }
    }

    [EnableMethod]
    public abstract class UGFUIForm : Entity
    {
        [BsonIgnore]
        private UIForm uiForm;
        [BsonIgnore]
        private CancellationTokenSourcePlus cts;
        [BsonIgnore]
        internal virtual AETMonoUGFUIForm UGFMono { get; set; }
        [BsonIgnore]
        public Transform CachedTransform { get; internal set; }

        public bool Available => this.uiForm != null && !this.uiForm.Logic.Available;
        public bool Visible
        {
            get
            {
                return this.uiForm != null && this.uiForm.Logic.Visible;
            }
            set
            {
                if (this.uiForm == null)
                {
                    Log.Warning("UI form is not opened.");
                    return;
                }
                this.uiForm.Logic.Visible = value;
            }
        }

        public override void Dispose()
        {
            if (!this.IsDisposed)
            {
                if (this.cts != null)
                {
                    this.cts.Cancel();
                    ObjectPool.Instance.Recycle(this.cts);
                    this.cts = null;
                }
                if (this.uiForm != null)
                {
                    GameEntry.UI.CloseUIForm(this.uiForm);
                    this.uiForm = null;
                }
            }
            base.Dispose();
        }

        public async UniTask OpenUIFormAsync(int uiFormTypeId)
        {
            if(this.cts == null)
            {
                this.cts = ObjectPool.Instance.Fetch<CancellationTokenSourcePlus>();
            }
            this.uiForm = await GameEntry.UI.OpenUIFormAsync(uiFormTypeId, ETMonoUGFUIFormData.Create(this), cancellationToken: this.cts.Token);
            if(this.uiForm == null)
            {
                throw new System.Exception($"UGFUIForm OpenUIFormAsync failed! uiFormTypeId:'{uiFormTypeId}'.");
            }
        }

        public void RefocusUIForm()
        {
            GameEntry.UI.RefocusUIForm(this.uiForm);
        }

        public void SetUIFormInstanceLocked(bool locked)
        {
            GameEntry.UI.SetUIFormInstanceLocked(this.uiForm, locked);
        }

        public void SetUIFormInstancePriority(int priority)
        {
            GameEntry.UI.SetUIFormInstancePriority(this.uiForm, priority);
        }

        public T AddChildUIWidget<T>(AETMonoUGFUIWidget etMonoWidget, bool isFromPool = false) where T : UGFUIWidget, IAwake
        {
            T widgetEntity = this.AddChild<T>(isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidget<T, A>(AETMonoUGFUIWidget etMonoWidget, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            T widgetEntity = this.AddChild<T, A>(a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidget<T, A, B>(AETMonoUGFUIWidget etMonoWidget, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            T widgetEntity = this.AddChild<T, A, B>(a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidget<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            T widgetEntity = this.AddChild<T, A, B, C>(a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidgetWithId<T>(AETMonoUGFUIWidget etMonoWidget, long id, bool isFromPool = false) where T : UGFUIWidget, IAwake
        {
            T widgetEntity = this.AddChildWithId<T>(id, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }
        public T AddChildUIWidgetWithId<T, A>(AETMonoUGFUIWidget etMonoWidget, long id, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            T widgetEntity = this.AddChildWithId<T, A>(id, a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidgetWithId<T, A, B>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            T widgetEntity = this.AddChildWithId<T, A, B>(id, a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddChildUIWidgetWithId<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            T widgetEntity = this.AddChildWithId<T, A, B, C>(id, a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T>(AETMonoUGFUIWidget etMonoWidget) where T : UGFUIWidget, IAwake, new()
        {
            T widgetEntity = this.AddComponent<T>();
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T, A>(AETMonoUGFUIWidget etMonoWidget, A a) where T : UGFUIWidget, IAwake<A>, new()
        {
            T widgetEntity = this.AddComponent<T, A>(a);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T, A, B>(AETMonoUGFUIWidget etMonoWidget, A a, B b) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            T widgetEntity = this.AddComponent<T, A, B>(a, b);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, A a, B b, C c) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            T widgetEntity = this.AddComponent<T, A, B, C>(a, b, c);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T>(AETMonoUGFUIWidget etMonoWidget, long id) where T : UGFUIWidget, IAwake, new()
        {
            T widgetEntity = this.AddComponentWithId<T>(id);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T, A>(AETMonoUGFUIWidget etMonoWidget, long id, A a) where T : UGFUIWidget, IAwake<A>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A>(id, a);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T, A, B>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A, B>(id, a, b);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidgetWithId<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, C c) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A, B, C>(id, a, b, c);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }
    }
}
