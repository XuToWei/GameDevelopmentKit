using System;

namespace ET
{
    public interface IUGFUIWidgetOnPause
    {
    }

    public interface IUGFUIWidgetOnPauseSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnPauseSystem<T> : SystemObject, IUGFUIWidgetOnPauseSystem where T : UGFUIWidget, IUGFUIWidgetOnPause
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnPauseSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnPauseSystem.Run(UGFUIWidget o)
        {
            this.UIWidgetOnPause((T)o);
        }

        protected abstract void UIWidgetOnPause(T self);
    }
}

