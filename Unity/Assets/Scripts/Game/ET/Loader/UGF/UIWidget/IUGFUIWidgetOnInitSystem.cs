using System;

namespace ET
{
    public interface IUGFUIWidgetOnInit
    {
    }

    public interface IUGFUIWidgetOnInitSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnInitSystem<T> : SystemObject, IUGFUIWidgetOnInitSystem where T : UGFUIWidget, IUGFUIWidgetOnInit
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnInitSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnInitSystem.Run(UGFUIWidget o)
        {
            this.UGFUIWidgetOnInit((T)o);
        }

        protected abstract void UGFUIWidgetOnInit(T self);
    }
}
