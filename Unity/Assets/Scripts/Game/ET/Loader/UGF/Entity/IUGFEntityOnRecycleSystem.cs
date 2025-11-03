using System;

namespace ET
{
    public interface IUGFEntityOnRecycle
    {
    }

    public interface IUGFEntityOnRecycleSystem : ISystemType
    {
        void Run(UGFEntity o);
    }

    [EntitySystem]
    public abstract class UGFEntityOnRecycleSystem<T> : SystemObject, IUGFEntityOnRecycleSystem where T : UGFEntity, IUGFEntityOnRecycle
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnRecycleSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnRecycleSystem.Run(UGFEntity o)
        {
            this.UGFEntityOnRecycle((T)o);
        }

        protected abstract void UGFEntityOnRecycle(T self);
    }
}

