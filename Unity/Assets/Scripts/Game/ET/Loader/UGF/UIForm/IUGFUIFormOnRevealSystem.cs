using System;

namespace ET
{
    public interface IUGFUIFormOnReveal
    {
    }

    public interface IUGFUIFormOnRevealSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnRevealSystem<T> : SystemObject, IUGFUIFormOnRevealSystem where T : UGFUIForm, IUGFUIFormOnReveal
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnRevealSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnRevealSystem.Run(UGFUIForm o)
        {
            this.UIFormOnReveal((T)o);
        }

        protected abstract void UIFormOnReveal(T self);
    }
}

