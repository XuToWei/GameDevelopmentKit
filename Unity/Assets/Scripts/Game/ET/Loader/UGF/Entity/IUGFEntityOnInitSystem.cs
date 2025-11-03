using System;

namespace ET
{
    public interface IUGFEntityOnInit
    {
    }

    public interface IUGFEntityOnInitSystem : ISystemType
    {
        void Run(UGFEntity o);
    }

    [EntitySystem]
    public abstract class UGFEntityOnInitSystem<T> : SystemObject, IUGFEntityOnInitSystem where T : UGFEntity, IUGFEntityOnInit
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnInitSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnInitSystem.Run(UGFEntity o)
        {
            this.UGFEntityOnInit((T)o);
        }

        protected abstract void UGFEntityOnInit(T self);
    }
}

