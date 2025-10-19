using System;

namespace ET
{
    public interface IUIFormOnResume
    {
    }

    public interface IUIFormOnResume<A>
    {
    }

    public interface IUIFormOnResume<A, B>
    {
    }

    public interface IUIFormOnResume<A, B, C>
    {
    }

    public interface IUIFormOnResume<A, B, C, D>
    {
    }

    public interface IUIFormOnResumeSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnResumeSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnResumeSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnResumeSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnResumeSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnResumeSystem<T> : SystemObject, IUIFormOnResumeSystem where T: Entity, IUIFormOnResume
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnResumeSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnResumeSystem.Run(Entity o)
        {
            this.UIFormOnResume((T)o);
        }

        protected abstract void UIFormOnResume(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnResumeSystem<T, A> : SystemObject, IUIFormOnResumeSystem<A> where T: Entity, IUIFormOnResume<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnResumeSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnResumeSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnResume((T)o, a);
        }

        protected abstract void UIFormOnResume(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnResumeSystem<T, A, B> : SystemObject, IUIFormOnResumeSystem<A, B> where T: Entity, IUIFormOnResume<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnResumeSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnResumeSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnResume((T)o, a, b);
        }

        protected abstract void UIFormOnResume(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnResumeSystem<T, A, B, C> : SystemObject, IUIFormOnResumeSystem<A, B, C> where T: Entity, IUIFormOnResume<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnResumeSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnResumeSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnResume((T)o, a, b, c);
        }

        protected abstract void UIFormOnResume(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnResumeSystem<T, A, B, C, D> : SystemObject, IUIFormOnResumeSystem<A, B, C, D> where T: Entity, IUIFormOnResume<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnResumeSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnResumeSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnResume((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnResume(T self, A a, B b, C c, D d);
    }
}