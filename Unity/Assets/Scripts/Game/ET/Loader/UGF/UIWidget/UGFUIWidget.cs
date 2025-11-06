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
        public override AETMonoUGFUIWidget UGFMono
        {
            get => base.UGFMono;
            internal set
            {
                base.UGFMono = value;
                View = base.UGFMono.GetComponent<T>();
            }
        }
    }

    [ChildOf(typeof(UGFUIForm))]
    [ComponentOf(typeof(UGFUIForm))]
    public abstract class UGFUIWidget : Entity
    {
        [BsonIgnore]
        public virtual AETMonoUGFUIWidget UGFMono { get; internal set; }
        [BsonIgnore]
        public Transform CachedTransform { get; internal set; }

        public bool Available => UGFMono != null && UGFMono.Available;
        public bool Visible
        {
            get
            {
                return UGFMono !=  null && UGFMono.Visible;
            }
            set
            {
                if (UGFMono == null)
                {
                    Log.Warning("UI widget is not opened.");
                    return;
                }
                UGFMono.Visible = value;
            }
        }

        public override void Dispose()
        {
            if (!this.IsDisposed)
            {
                if (this.Available)
                {
                    this.UGFMono.Close();
                }
            }
        }
        
        public void Open()
        {
            this.UGFMono.Open();
        }

        public void DynamicOpen()
        {
            this.UGFMono.DynamicOpen();
        }

        public void Close()
        {
            this.UGFMono.Close();
        }
    }
}
