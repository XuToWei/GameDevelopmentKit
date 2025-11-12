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
        public Transform CachedTransform { get; internal set; }
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
            base.Dispose();
            if (!isDisposed)
            {
                if (this.Available)
                {
                    this.UGFMono.Close();
                }
                if (this.UGFMono != null && this.UGFMono.Has())
                {
                    this.UGFMono.Remove();
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

        public void Remove()
        {
            this.UGFMono.Remove();
        }
    }
}
