using System;

namespace ET
{
    public interface IUGFUIFormOnClose
    {
    }

    public interface IUGFUIFormOnCloseSystem : ISystemType
    {
        void Run(UGFUIForm o, bool isShutdown);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnCloseSystem<T> : SystemObject, IUGFUIFormOnCloseSystem where T : UGFUIForm, IUGFUIFormOnClose
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnCloseSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnCloseSystem.Run(UGFUIForm o, bool isShutdown)
        {
            this.UGFUIFormOnClose((T)o, isShutdown);
        }

        protected abstract void UGFUIFormOnClose(T self, bool isShutdown);
    }
}

