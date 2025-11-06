using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;
using UnityGameFramework.Extension;
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

        public async UniTask<T> AddChildUIWidgetAsync<T>(string widgetAssetName, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidget<T>(etMonoWidget, isFromPool);
        }

        public T AddChildUIWidget<T, A>(AETMonoUGFUIWidget etMonoWidget, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            T widgetEntity = this.AddChild<T, A>(a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddChildUIWidgetAsync<T, A>(string widgetAssetName, A a, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidget<T, A>(etMonoWidget, a, isFromPool);
        }

        public T AddChildUIWidget<T, A, B>(AETMonoUGFUIWidget etMonoWidget, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            T widgetEntity = this.AddChild<T, A, B>(a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddChildUIWidgetAsync<T, A, B>(string widgetAssetName, A a, B b, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidget<T, A, B>(etMonoWidget, a, b, isFromPool);
        }

        public T AddChildUIWidget<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            T widgetEntity = this.AddChild<T, A, B, C>(a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddChildUIWidgetAsync<T, A, B, C>(string widgetAssetName, A a, B b, C c, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidget<T, A, B, C>(etMonoWidget, a, b, c, isFromPool);
        }

        public T AddChildUIWidgetWithId<T>(AETMonoUGFUIWidget etMonoWidget, long id, bool isFromPool = false) where T : UGFUIWidget, IAwake
        {
            T widgetEntity = this.AddChildWithId<T>(id, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddChildUIWidgetWithIdAsync<T>(string widgetAssetName, long id, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidgetWithId<T>(etMonoWidget, id, isFromPool);
        }

        public T AddChildUIWidgetWithId<T, A>(AETMonoUGFUIWidget etMonoWidget, long id, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            T widgetEntity = this.AddChildWithId<T, A>(id, a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddChildUIWidgetWithIdAsync<T, A>(string widgetAssetName, long id, A a, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidgetWithId<T, A>(etMonoWidget, id, a, isFromPool);
        }

        public T AddChildUIWidgetWithId<T, A, B>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            T widgetEntity = this.AddChildWithId<T, A, B>(id, a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddChildUIWidgetWithIdAsync<T, A, B>(string widgetAssetName, long id, A a, B b, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidgetWithId<T, A, B>(etMonoWidget, id, a, b, isFromPool);
        }

        public T AddChildUIWidgetWithId<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            T widgetEntity = this.AddChildWithId<T, A, B, C>(id, a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddChildUIWidgetWithIdAsync<T, A, B, C>(string widgetAssetName, long id, A a, B b, C c, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddChildUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddChildUIWidgetWithId<T, A, B, C>(etMonoWidget, id, a, b, c, isFromPool);
        }

        public T AddComponentUIWidget<T>(AETMonoUGFUIWidget etMonoWidget, bool isFromPool = false) where T : UGFUIWidget, IAwake, new()
        {
            T widgetEntity = this.AddComponent<T>(isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetAsync<T>(string widgetAssetName, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidget<T>(etMonoWidget, isFromPool);
        }

        public T AddComponentUIWidget<T, A>(AETMonoUGFUIWidget etMonoWidget, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>, new()
        {
            T widgetEntity = this.AddComponent<T, A>(a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetAsync<T, A>(string widgetAssetName, A a, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidget<T, A>(etMonoWidget, a, isFromPool);
        }

        public T AddComponentUIWidget<T, A, B>(AETMonoUGFUIWidget etMonoWidget, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            T widgetEntity = this.AddComponent<T, A, B>(a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetAsync<T, A, B>(string widgetAssetName, A a, B b, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidget<T, A, B>(etMonoWidget, a, b, isFromPool);
        }

        public T AddComponentUIWidget<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            T widgetEntity = this.AddComponent<T, A, B, C>(a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetAsync<T, A, B, C>(string widgetAssetName, A a, B b, C c, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidget<T, A, B, C>(etMonoWidget, a, b, c, isFromPool);
        }

        public T AddComponentUIWidgetWithId<T>(AETMonoUGFUIWidget etMonoWidget, long id, bool isFromPool = false) where T : UGFUIWidget, IAwake, new()
        {
            T widgetEntity = this.AddComponentWithId<T>(id, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetWithIdAsync<T>(string widgetAssetName, long id, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidgetWithId<T>(etMonoWidget, id, isFromPool);
        }

        public T AddComponentUIWidgetWithId<T, A>(AETMonoUGFUIWidget etMonoWidget, long id, A a, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A>(id, a, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetWithIdAsync<T, A>(string widgetAssetName, long id, A a, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A>, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidgetWithId<T, A>(etMonoWidget, id, a, isFromPool);
        }

        public T AddComponentUIWidgetWithId<T, A, B>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A, B>(id, a, b, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetWithIdAsync<T, A, B>(string widgetAssetName, long id, A a, B b, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B>, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidgetWithId<T, A, B>(etMonoWidget, id, a, b, isFromPool);
        }

        public T AddComponentUIWidgetWithId<T, A, B, C>(AETMonoUGFUIWidget etMonoWidget, long id, A a, B b, C c, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            T widgetEntity = this.AddComponentWithId<T, A, B, C>(id, a, b, c, isFromPool);
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public async UniTask<T> AddComponentUIWidgetWithIdAsync<T, A, B, C>(string widgetAssetName, long id, A a, B b, C c, CancellationToken cancellationToken = default, bool isFromPool = false) where T : UGFUIWidget, IAwake<A, B, C>, new()
        {
            GameObject etMonoWidgetGameObject = await GameEntry.Resource.LoadAssetAsync<GameObject>(widgetAssetName, cancellationToken: cancellationToken);
            if(etMonoWidgetGameObject == null)
            {
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not found.");
            }
            AETMonoUGFUIWidget etMonoWidget = etMonoWidgetGameObject.GetComponent<AETMonoUGFUIWidget>();
            if(etMonoWidget == null)
            {
                GameEntry.Resource.UnloadAsset(etMonoWidgetGameObject);
                throw new Exception($"AddComponentUIWidgetWithIdAsync failed! widgetAssetName:'{widgetAssetName}' is not a UGFUIWidget.");
            }
            return AddComponentUIWidgetWithId<T, A, B, C>(etMonoWidget, id, a, b, c, isFromPool);
        }
    }
}
