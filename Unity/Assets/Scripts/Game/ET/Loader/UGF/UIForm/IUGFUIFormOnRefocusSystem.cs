using System;

namespace ET
{
    public interface IUGFUIFormOnRefocus
    {
    }

    public interface IUGFUIFormOnRefocusSystem : ISystemType
    {
        void Run(UGFUIForm o);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnRefocusSystem<T> : SystemObject, IUGFUIFormOnRefocusSystem where T : UGFUIForm, IUGFUIFormOnRefocus
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnRefocusSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnRefocusSystem.Run(UGFUIForm o)
        {
            this.UIFormOnRefocus((T)o);
        }

        protected abstract void UIFormOnRefocus(T self);
    }
}

