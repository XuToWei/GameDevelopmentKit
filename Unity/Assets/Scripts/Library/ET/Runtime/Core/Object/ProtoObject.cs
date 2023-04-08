using System;
using System.ComponentModel;

namespace ET
{
    public abstract class ProtoObject : Object, ISupportInitialize, IDisposable
    {
        public object Clone()
        {
            byte[] bytes = SerializeHelper.Serialize(this);
            return SerializeHelper.Deserialize(this.GetType(), bytes, 0, bytes.Length);
        }

        public virtual void BeginInit()
        {
        }


        public virtual void EndInit()
        {
        }

        public virtual void AfterEndInit()
        {
        }

        public virtual void Dispose()
        {
            ObjectPool.Instance.Recycle(this);
        }
    }
}