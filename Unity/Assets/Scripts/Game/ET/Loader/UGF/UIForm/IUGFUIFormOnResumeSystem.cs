using System;

namespace ET
{
    public interface IUGFUIFormOnResume
    {
    }

    public interface IUGFUIFormOnResumeSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnResumeSystem<T> : SystemObject, IUGFUIFormOnResumeSystem where T : UGFUIForm, IUGFUIFormOnResume
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnResumeSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnResumeSystem.Run(UGFUIForm o)
        {
            this.UGFUIFormOnResume((T)o);
        }

        protected abstract void UGFUIFormOnResume(T self);
    }
}

