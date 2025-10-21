using System;

namespace ET
{
    public interface IUGFUIWidgetOnUpdate
    {
    }

    public interface IUGFUIWidgetOnUpdateSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnUpdateSystem<T> : SystemObject, IUGFUIWidgetOnUpdateSystem where T : UGFUIWidget, IUGFUIWidgetOnUpdate
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnUpdateSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnUpdateSystem.Run(UGFUIWidget o)
        {
            this.UIWidgetOnUpdate((T)o);
        }

        protected abstract void UIWidgetOnUpdate(T self);
    }
}

