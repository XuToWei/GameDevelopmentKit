using System;

namespace ET
{
    public interface IUIFormOnReveal
    {
    }

    public interface IUIFormOnReveal<A>
    {
    }

    public interface IUIFormOnReveal<A, B>
    {
    }

    public interface IUIFormOnReveal<A, B, C>
    {
    }

    public interface IUIFormOnReveal<A, B, C, D>
    {
    }

    public interface IUIFormOnRevealSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnRevealSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnRevealSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnRevealSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnRevealSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnRevealSystem<T> : SystemObject, IUIFormOnRevealSystem where T: Entity, IUIFormOnReveal
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRevealSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRevealSystem.Run(Entity o)
        {
            this.UIFormOnReveal((T)o);
        }

        protected abstract void UIFormOnReveal(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnRevealSystem<T, A> : SystemObject, IUIFormOnRevealSystem<A> where T: Entity, IUIFormOnReveal<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRevealSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRevealSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnReveal((T)o, a);
        }

        protected abstract void UIFormOnReveal(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnRevealSystem<T, A, B> : SystemObject, IUIFormOnRevealSystem<A, B> where T: Entity, IUIFormOnReveal<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRevealSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRevealSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnReveal((T)o, a, b);
        }

        protected abstract void UIFormOnReveal(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnRevealSystem<T, A, B, C> : SystemObject, IUIFormOnRevealSystem<A, B, C> where T: Entity, IUIFormOnReveal<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRevealSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRevealSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnReveal((T)o, a, b, c);
        }

        protected abstract void UIFormOnReveal(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnRevealSystem<T, A, B, C, D> : SystemObject, IUIFormOnRevealSystem<A, B, C, D> where T: Entity, IUIFormOnReveal<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRevealSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRevealSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnReveal((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnReveal(T self, A a, B b, C c, D d);
    }
}