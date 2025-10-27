using System;

namespace ET
{
    public interface IUGFUIWidgetOnDepthChanged
    {
    }

    public interface IUGFUIWidgetOnDepthChangedSystem : ISystemType
    {
        void Run(UGFUIWidget o, int uiGroupDepth, int depthInUIGroup);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnDepthChangedSystem<T> : SystemObject, IUGFUIWidgetOnDepthChangedSystem where T : UGFUIWidget, IUGFUIWidgetOnDepthChanged
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnDepthChangedSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnDepthChangedSystem.Run(UGFUIWidget o, int uiGroupDepth, int depthInUIGroup)
        {
            this.UGFUIWidgetOnDepthChanged((T)o, uiGroupDepth, depthInUIGroup);
        }

        protected abstract void UGFUIWidgetOnDepthChanged(T self, int uiGroupDepth, int depthInUIGroup);
    }
}
