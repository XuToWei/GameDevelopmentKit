using System;

namespace ET
{
    public interface IUGFUIFormOnDepthChanged
    {
    }

    public interface IUGFUIFormOnDepthChangedSystem : ISystemType
    {
        void Run(UGFUIForm o, int uiGroupDepth, int depthInUIGroup);
    }

    [EntitySystem]
    public abstract class UGFUIFormOnDepthChangedSystem<T> : SystemObject, IUGFUIFormOnDepthChangedSystem where T : UGFUIForm, IUGFUIFormOnDepthChanged
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIFormOnDepthChangedSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIFormOnDepthChangedSystem.Run(UGFUIForm o, int uiGroupDepth, int depthInUIGroup)
        {
            this.UGFUIFormOnDepthChanged((T)o, uiGroupDepth, depthInUIGroup);
        }

        protected abstract void UGFUIFormOnDepthChanged(T self, int uiGroupDepth, int depthInUIGroup);
    }
}
