using System;

namespace ET
{
    public interface IUIFormOnRefocus
    {
    }

    public interface IUIFormOnRefocus<A>
    {
    }

    public interface IUIFormOnRefocus<A, B>
    {
    }

    public interface IUIFormOnRefocus<A, B, C>
    {
    }

    public interface IUIFormOnRefocus<A, B, C, D>
    {
    }

    public interface IUIFormOnRefocusSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnRefocusSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnRefocusSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnRefocusSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnRefocusSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnRefocusSystem<T> : SystemObject, IUIFormOnRefocusSystem where T: Entity, IUIFormOnRefocus
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRefocusSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRefocusSystem.Run(Entity o)
        {
            this.UIFormOnRefocus((T)o);
        }

        protected abstract void UIFormOnRefocus(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnRefocusSystem<T, A> : SystemObject, IUIFormOnRefocusSystem<A> where T: Entity, IUIFormOnRefocus<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRefocusSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRefocusSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnRefocus((T)o, a);
        }

        protected abstract void UIFormOnRefocus(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnRefocusSystem<T, A, B> : SystemObject, IUIFormOnRefocusSystem<A, B> where T: Entity, IUIFormOnRefocus<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRefocusSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRefocusSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnRefocus((T)o, a, b);
        }

        protected abstract void UIFormOnRefocus(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnRefocusSystem<T, A, B, C> : SystemObject, IUIFormOnRefocusSystem<A, B, C> where T: Entity, IUIFormOnRefocus<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRefocusSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRefocusSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnRefocus((T)o, a, b, c);
        }

        protected abstract void UIFormOnRefocus(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnRefocusSystem<T, A, B, C, D> : SystemObject, IUIFormOnRefocusSystem<A, B, C, D> where T: Entity, IUIFormOnRefocus<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRefocusSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRefocusSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnRefocus((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnRefocus(T self, A a, B b, C c, D d);
    }
}