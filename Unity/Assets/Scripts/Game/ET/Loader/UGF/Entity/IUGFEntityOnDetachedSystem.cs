using System;
using UnityGameFramework.Runtime;

namespace ET
{
    public interface IUGFEntityOnDetached
    {
    }

    public interface IUGFEntityOnDetachedSystem : ISystemType
    {
        void Run(UGFEntity o, UGFEntity childEntity);
    }

    [EntitySystem]
    public abstract class UGFEntityOnDetachedSystem<T> : SystemObject, IUGFEntityOnDetachedSystem where T : UGFEntity, IUGFEntityOnDetached
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnDetachedSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnDetachedSystem.Run(UGFEntity o, UGFEntity childEntity)
        {
            this.UGFEntityOnDetached((T)o, childEntity);
        }

        protected abstract void UGFEntityOnDetached(T self, UGFEntity childEntity);
    }
}

