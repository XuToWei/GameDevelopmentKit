using System;

namespace ET
{
    public interface IUGFUIWidgetOnOpen
    {
    }

    public interface IUGFUIWidgetOnOpenSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnOpenSystem<T> : SystemObject, IUGFUIWidgetOnOpenSystem where T : UGFUIWidget, IUGFUIWidgetOnOpen
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnOpenSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnOpenSystem.Run(UGFUIWidget o)
        {
            this.UIWidgetOnOpen((T)o);
        }

        protected abstract void UIWidgetOnOpen(T self);
    }
}

