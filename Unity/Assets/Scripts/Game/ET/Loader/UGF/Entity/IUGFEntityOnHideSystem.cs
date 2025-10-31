using System;

namespace ET
{
    public interface IUGFEntityOnHide
    {
    }

    public interface IUGFEntityOnHideSystem : ISystemType
    {
        void Run(UGFEntity o, bool isShutdown);
    }

    [EntitySystem]
    public abstract class UGFEntityOnHideSystem<T> : SystemObject, IUGFEntityOnHideSystem where T : UGFEntity, IUGFEntityOnHide
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnHideSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnHideSystem.Run(UGFEntity o, bool isShutdown)
        {
            this.UGFEntityOnHide((T)o, isShutdown);
        }

        protected abstract void UGFEntityOnHide(T self, bool isShutdown);
    }
}

