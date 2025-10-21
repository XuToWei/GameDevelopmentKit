using System;

namespace ET
{
    public interface IUGFUIWidgetOnReveal
    {
    }

    public interface IUGFUIWidgetOnRevealSystem : ISystemType
    {
        void Run(UGFUIWidget o);
    }

    [EntitySystem]
    public abstract class UGFUIWidgetOnRevealSystem<T> : SystemObject, IUGFUIWidgetOnRevealSystem where T : UGFUIWidget, IUGFUIWidgetOnReveal
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUGFUIWidgetOnRevealSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUGFUIWidgetOnRevealSystem.Run(UGFUIWidget o)
        {
            this.UIWidgetOnReveal((T)o);
        }

        protected abstract void UIWidgetOnReveal(T self);
    }
}

