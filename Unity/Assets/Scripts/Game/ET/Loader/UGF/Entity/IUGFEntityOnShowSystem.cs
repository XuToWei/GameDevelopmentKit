using System;

namespace ET
{
    public interface IUGFEntityOnShow
    {
    }

    public interface IUGFEntityOnShowSystem : ISystemType
    {
        void Run(UGFEntity o);
    }

    [EntitySystem]
    public abstract class UGFEntityOnShowSystem<T> : SystemObject, IUGFEntityOnShowSystem where T : UGFEntity, IUGFEntityOnShow
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnShowSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnShowSystem.Run(UGFEntity o)
        {
            this.UGFEntityOnShow((T)o);
        }

        protected abstract void UGFEntityOnShow(T self);
    }
}

