using System;
using Cysharp.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace ET
{
    [EnableMethod]
    public abstract class UGFUIWidget<T> : UGFUIWidget where T : MonoBehaviour
    {
        [BsonIgnore]
        public T View { get; private set; }

        [BsonIgnore]
        internal override AETMonoUGFUIWidget UGFMono
        {
            get => base.UGFMono;
            set
            {
                if (value == null)
                {
                    base.UGFMono = null;
                    this.View = null;
                }
                else
                {
                    base.UGFMono = value;
                    this.View = base.UGFMono.GetComponent<T>();
                }
            }
        }
    }

    [ChildOf(typeof(UGFUIForm))]
    [ComponentOf(typeof(UGFUIForm))]
    public abstract class UGFUIWidget : Entity
    {
        [BsonIgnore]
        internal virtual AETMonoUGFUIWidget UGFMono { get; set; }
        [BsonIgnore]
        public UGFUIWidget ParentUGFUIWidget { get; internal set; }
        [BsonIgnore]
        public RectTransform CachedRectTransform { get; internal set; }
        [BsonIgnore]
        public bool Available =>  this.UGFMono != null && this.UGFMono.Available;
        [BsonIgnore]
        public bool Visible
        {
            get
            {
                return this.UGFMono != null && this.UGFMono.Visible;
            }
            set
            {
                if (this.UGFMono == null)
                {
                    Log.Warning("UI widget is not opened.");
                    return;
                }
                this.UGFMono.Visible = value;
            }
        }

        public override void Dispose()
        {
            bool isDisposed = this.IsDisposed;
            if (!isDisposed)
            {
                if (this.Available)
                {
                    this.Close();
                    this.Remove();
                }
            }
            base.Dispose();
        }

        public void OpenAllUIWidgets()
        {
            this.UGFMono.OpenAllUIWidgets();
        }

        public void Open()
        {
            this.UGFMono.Open();
        }

        public void TryOpen()
        {
            this.UGFMono.TryOpen();
        }

        public void DynamicOpen()
        {
            this.UGFMono.DynamicOpen();
        }

        public void TryDynamicOpen()
        {
            this.UGFMono.TryDynamicOpen();
        }

        public void Close()
        {
            this.UGFMono.Close();
        }

        public void TryClose()
        {
            this.UGFMono.TryClose();
        }

        internal bool Has()
        {
            return this.UGFMono.Has();
        }

        public void Remove()
        {
            this.UGFMono.Remove();
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
