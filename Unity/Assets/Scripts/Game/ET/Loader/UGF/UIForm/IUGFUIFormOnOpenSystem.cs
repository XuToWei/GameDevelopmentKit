using System;

namespace ET
{
    public interface IUGFUIFormOnOpen
    {
    }

    public interface IUGFUIFormOnOpenSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnOpenSystem<T> : SystemObject, IUGFUIFormOnOpenSystem where T : UGFUIForm, IUGFUIFormOnOpen
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnOpenSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnOpenSystem.Run(UGFUIForm o)
        {
            this.UIFormOnOpen((T)o);
        }

        protected abstract void UIFormOnOpen(T self);
    }
}