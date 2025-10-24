using System;

namespace ET
{
    public interface IUGFUIWidgetOnUpdate
    {
    }

    public interface IUGFUIWidgetOnUpdateSystem : ISystemType
    {
        void Run(UGFUIWidget o, float elapseSeconds, float realElapseSeconds);
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

        void IUGFUIWidgetOnUpdateSystem.Run(UGFUIWidget o, float elapseSeconds, float realElapseSeconds)
        {
            this.UGFUIWidgetOnUpdate((T)o, elapseSeconds, realElapseSeconds);
        }

        protected abstract void UGFUIWidgetOnUpdate(T self, float elapseSeconds, float realElapseSeconds);
    }
}

