using System;

namespace ET
{
    public interface IUGFUIFormOnUpdate
    {
    }

    public interface IUGFUIFormOnUpdateSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnUpdateSystem<T> : SystemObject, IUGFUIFormOnUpdateSystem where T : UGFUIForm, IUGFUIFormOnUpdate
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnUpdateSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnUpdateSystem.Run(UGFUIForm o)
        {
            this.UIFormOnUpdate((T)o);
        }

        protected abstract void UIFormOnUpdate(T self);
    }
}

