using System;

namespace ET
{
    public interface IUGFUIFormOnPause
    {
    }

    public interface IUGFUIFormOnPauseSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnPauseSystem<T> : SystemObject, IUGFUIFormOnPauseSystem where T : UGFUIForm, IUGFUIFormOnPause
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnPauseSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnPauseSystem.Run(UGFUIForm o)
        {
            this.UGFUIFormOnPause((T)o);
        }

        protected abstract void UGFUIFormOnPause(T self);
    }
}

