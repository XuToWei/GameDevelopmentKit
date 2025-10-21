using System;

namespace ET
{
    public interface IUGFUIFormOnInit
    {
    }

    public interface IUGFUIFormOnInitSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnInitSystem<T> : SystemObject, IUGFUIFormOnInitSystem where T : UGFUIForm, IUGFUIFormOnInit
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnInitSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnInitSystem.Run(UGFUIForm o)
        {
            this.UIFormOnInit((T)o);
        }

        protected abstract void UIFormOnInit(T self);
    }
}

