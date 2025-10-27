using UnityEngine;

namespace ET
{
    [EnableMethod]
    public abstract class UGFUIWidget<T> : UGFUIWidget where T : ETMonoUGFUIWidget
    {
        public T Mono { get; private set; }

        private ETMonoUGFUIWidget etMono;
        public override ETMonoUGFUIWidget ETMono
        {
            get => etMono;
            internal set
            {
                etMono = value;
                Mono = (T)etMono;
            }
        }
    }

    [ComponentOf(typeof(UGFUIForm))]
    [ChildOf(typeof(UGFUIForm))]
    public abstract class UGFUIWidget : Entity, IAwake, IDestroy
    {
        public virtual ETMonoUGFUIWidget ETMono { get; internal set; }
        public Transform CachedTransform { get; internal set; }
        public bool IsOpen => ETMono != null;
        public bool Available => ETMono != null && ETMono.Available;

        public bool Visible
        {
            get
            {
                return ETMono !=  null && ETMono.Visible;
            }
            set
            {
                if (ETMono == null)
                {
                    Log.Warning("UI widget is not opened.");
                    return;
                }
                ETMono.Visible = value;
            }
        }

        public override void Dispose()
        {
            if (!this.IsDisposed)
            {
                if (this.Available)
                {
                    this.ETMono.Close();
                }
            }
        }
        
        public void Open()
        {
            this.ETMono.Open();
        }

        public void DynamicOpen()
        {
            this.ETMono.DynamicOpen();
        }

        public void Close()
        {
            this.ETMono.Close();
        }
    }
}
