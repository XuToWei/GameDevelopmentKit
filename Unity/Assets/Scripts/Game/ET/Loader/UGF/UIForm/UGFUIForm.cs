using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET
{
    [EnableMethod]
    public abstract class UGFUIForm : Entity, IAwake, IDestroy
    {
        private UIForm uiForm;
        private CancellationTokenSource cts;

        public ETMonoUGFUIForm ETMono { get; internal set; }
        public Transform CachedTransform { get; internal set; }
        public bool IsOpen => this.uiForm != null;
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
                    this.cts.Dispose();
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
            this.cts = new CancellationTokenSource();
            this.uiForm = await GameEntry.UI.OpenUIFormAsync(uiFormTypeId, ETMonoUGFUIFormData.Create(this), this.cts.Token);
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

        public T AddUIWidget<T>(ETMonoUGFUIWidget etMonoWidget) where T : UGFUIWidget
        {
            T widgetEntity = this.AddChild<T>();
            this.ETMono.AddUIWidget(etMonoWidget, ETMonoUGFUIWidgetData.Create(this, widgetEntity));
            return widgetEntity;
        }
    }
}