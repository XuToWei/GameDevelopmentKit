using System;
using UnityGameFramework.Runtime;

namespace ET
{
    public interface IUGFEntityOnDetachFrom
    {
    }

    public interface IUGFEntityOnDetachFromSystem : ISystemType
    {
        void Run(UGFEntity o, UGFEntity parentEntity);
    }

    [EntitySystem]
    public abstract class UGFEntityOnDetachFromSystem<T> : SystemObject, IUGFEntityOnDetachFromSystem where T : UGFEntity, IUGFEntityOnDetachFrom
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnDetachFromSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnDetachFromSystem.Run(UGFEntity o, UGFEntity parentEntity)
        {
            this.UGFEntityOnDetachFrom((T)o, parentEntity);
        }

        protected abstract void UGFEntityOnDetachFrom(T self, UGFEntity parentEntity);
    }
}

