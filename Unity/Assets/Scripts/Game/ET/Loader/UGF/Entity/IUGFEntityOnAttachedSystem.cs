using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace ET
{
    public interface IUGFEntityOnAttached
    {
    }

    public interface IUGFEntityOnAttachedSystem : ISystemType
    {
        void Run(UGFEntity o, UGFEntity childEntity, Transform parentTransform);
    }

    [EntitySystem]
    public abstract class UGFEntityOnAttachedSystem<T> : SystemObject, IUGFEntityOnAttachedSystem where T : UGFEntity, IUGFEntityOnAttached
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFEntityOnAttachedSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFEntityOnAttachedSystem.Run(UGFEntity o, UGFEntity childEntity, Transform parentTransform)
        {
            this.UGFEntityOnAttached((T)o, childEntity, parentTransform);
        }

        protected abstract void UGFEntityOnAttached(T self, UGFEntity childEntity, Transform parentTransform);
    }
}

