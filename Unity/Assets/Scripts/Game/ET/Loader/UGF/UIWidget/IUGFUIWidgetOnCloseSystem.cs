using System;

namespace ET
{
    public interface IUGFUIWidgetOnClose
    {
    }

    public interface IUGFUIWidgetOnCloseSystem : ISystemType
    {
        void Run(UGFUIWidget o, bool isShutdown);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnCloseSystem<T> : SystemObject, IUGFUIWidgetOnCloseSystem where T : UGFUIWidget, IUGFUIWidgetOnClose
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnCloseSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnCloseSystem.Run(UGFUIWidget o, bool isShutdown)
        {
            this.UIWidgetOnClose((T)o, isShutdown);
        }

        protected abstract void UIWidgetOnClose(T self, bool isShutdown);
    }
}

