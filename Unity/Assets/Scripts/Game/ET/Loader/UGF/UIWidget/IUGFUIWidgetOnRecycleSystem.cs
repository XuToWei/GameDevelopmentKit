using System;

namespace ET
{
    public interface IUGFUIWidgetOnRecycle
    {
    }

    public interface IUGFUIWidgetOnRecycleSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnRecycleSystem<T> : SystemObject, IUGFUIWidgetOnRecycleSystem where T : UGFUIWidget, IUGFUIWidgetOnRecycle
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnRecycleSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnRecycleSystem.Run(UGFUIWidget o)
        {
            this.UGFUIWidgetOnRecycle((T)o);
        }

        protected abstract void UGFUIWidgetOnRecycle(T self);
    }
}
