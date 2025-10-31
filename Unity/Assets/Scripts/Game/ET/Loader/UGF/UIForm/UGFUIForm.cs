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
    public abstract class UGFUIForm : Entity, IAwake, IDestroy 
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

        public T AddChildUIWidget<T>(AETMonoUGFUIWidget etMonoWidget) where T : UGFUIWidget
        {
            T widgetEntity = this.AddChild<T>();
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }

        public T AddComponentUIWidget<T>(AETMonoUGFUIWidget etMonoWidget) where T : UGFUIWidget, new()
        {
            T widgetEntity = this.AddComponent<T>();
            this.UGFMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }
    }
}
