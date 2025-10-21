using System;

namespace ET
{
    public interface IUGFUIWidgetOnRefocus
    {
    }

    public interface IUGFUIWidgetOnRefocusSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnRefocusSystem<T> : SystemObject, IUGFUIWidgetOnRefocusSystem where T : UGFUIWidget, IUGFUIWidgetOnRefocus
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnRefocusSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnRefocusSystem.Run(UGFUIWidget o)
        {
            this.UIWidgetOnRefocus((T)o);
        }

        protected abstract void UIWidgetOnRefocus(T self);
    }
}

