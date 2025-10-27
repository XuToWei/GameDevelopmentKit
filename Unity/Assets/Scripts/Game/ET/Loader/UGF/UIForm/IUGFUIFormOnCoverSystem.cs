using System;

namespace ET
{
    public interface IUGFUIFormOnCover
    {
    }

    public interface IUGFUIFormOnCoverSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnCoverSystem<T> : SystemObject, IUGFUIFormOnCoverSystem where T : UGFUIForm, IUGFUIFormOnCover
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnCoverSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnCoverSystem.Run(UGFUIForm o)
        {
            this.UGFUIFormOnCover((T)o);
        }

        protected abstract void UGFUIFormOnCover(T self);
    }
}
