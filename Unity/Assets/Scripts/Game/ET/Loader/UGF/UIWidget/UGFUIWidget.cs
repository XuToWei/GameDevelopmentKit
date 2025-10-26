using UnityEngine;

namespace ET
{
    [ChildOf(typeof(UGFUIForm))]
    public abstract class UGFUIWidget : Entity, IAwake, IDestroy
    {
        public ETMonoUGFUIWidget ETMono { get; internal set; }
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
                this.ETMono.Close();
            }
        }
    }
}