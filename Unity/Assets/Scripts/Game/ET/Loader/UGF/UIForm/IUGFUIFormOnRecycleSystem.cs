using System;

namespace ET
{
    public interface IUGFUIFormOnRecycle
    {
    }

    public interface IUGFUIFormOnRecycleSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnRecycleSystem<T> : SystemObject, IUGFUIFormOnRecycleSystem where T : UGFUIForm, IUGFUIFormOnRecycle
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnRecycleSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnRecycleSystem.Run(UGFUIForm o)
        {
            this.UGFUIFormOnRecycle((T)o);
        }

        protected abstract void UGFUIFormOnRecycle(T self);
    }
}

