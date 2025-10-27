using System;

namespace ET
{
    public interface IUGFUIWidgetOnCover
    {
    }

    public interface IUGFUIWidgetOnCoverSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnCoverSystem<T> : SystemObject, IUGFUIWidgetOnCoverSystem where T : UGFUIWidget, IUGFUIWidgetOnCover
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnCoverSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnCoverSystem.Run(UGFUIWidget o)
        {
            this.UGFUIWidgetOnCover((T)o);
        }

        protected abstract void UGFUIWidgetOnCover(T self);
    }
}
