using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ET
{
    public interface IUGFEntityOnAttachTo
    {
    }

    public interface IUGFEntityOnAttachToSystem : ISystemType
    {
        void Run(UGFEntity o, UGFEntity parentEntity, Transform parentTransform);
    }

    [EntitySystem]
    public abstract class UGFEntityOnAttachToSystem<T> : SystemObject, IUGFEntityOnAttachToSystem where T : UGFEntity, IUGFEntityOnAttachTo
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnAttachToSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnAttachToSystem.Run(UGFEntity o, UGFEntity parentEntity, Transform parentTransform)
        {
            this.UGFEntityOnAttachTo((T)o, parentEntity, parentTransform);
        }

        protected abstract void UGFEntityOnAttachTo(T self, UGFEntity parentEntity, Transform parentTransform);
    }
}

