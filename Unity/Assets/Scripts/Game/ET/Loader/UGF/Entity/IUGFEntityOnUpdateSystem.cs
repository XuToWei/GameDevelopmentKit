using System;

namespace ET
{
    public interface IUGFEntityOnUpdate
    {
    }

    public interface IUGFEntityOnUpdateSystem : ISystemType
    {
        void Run(UGFEntity o, float elapseSeconds, float realElapseSeconds);
    }

    [EntitySystem]
    public abstract class UGFEntityOnUpdateSystem<T> : SystemObject, IUGFEntityOnUpdateSystem where T : UGFEntity, IUGFEntityOnUpdate
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnUpdateSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnUpdateSystem.Run(UGFEntity o, float elapseSeconds, float realElapseSeconds)
        {
            this.UGFEntityOnUpdate((T)o, elapseSeconds, realElapseSeconds);
        }

        protected abstract void UGFEntityOnUpdate(T self, float elapseSeconds, float realElapseSeconds);
    }
}

