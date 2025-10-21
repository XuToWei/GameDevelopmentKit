using System;

namespace ET
{
    public interface IUGFUIWidgetOnResume
    {
    }

    public interface IUGFUIWidgetOnResumeSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnResumeSystem<T> : SystemObject, IUGFUIWidgetOnResumeSystem where T : UGFUIWidget, IUGFUIWidgetOnResume
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnResumeSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnResumeSystem.Run(UGFUIWidget o)
        {
            this.UIWidgetOnResume((T)o);
        }

        protected abstract void UIWidgetOnResume(T self);
    }
}

