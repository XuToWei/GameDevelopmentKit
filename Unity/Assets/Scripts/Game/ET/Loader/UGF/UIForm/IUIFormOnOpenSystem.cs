using System;

namespace ET
{
    public interface IUIFormOnOpen
    {
    }

    public interface IUIFormOnOpen<A>
    {
    }

    public interface IUIFormOnOpen<A, B>
    {
    }

    public interface IUIFormOnOpen<A, B, C>
    {
    }

    public interface IUIFormOnOpen<A, B, C, D>
    {
    }

    public interface IUIFormOnOpenSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnOpenSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnOpenSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnOpenSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnOpenSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnOpenSystem<T> : SystemObject, IUIFormOnOpenSystem where T: Entity, IUIFormOnOpen
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnOpenSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnOpenSystem.Run(Entity o)
        {
            this.UIFormOnOpen((T)o);
        }

        protected abstract void UIFormOnOpen(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnOpenSystem<T, A> : SystemObject, IUIFormOnOpenSystem<A> where T: Entity, IUIFormOnOpen<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnOpenSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnOpenSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnOpen((T)o, a);
        }

        protected abstract void UIFormOnOpen(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnOpenSystem<T, A, B> : SystemObject, IUIFormOnOpenSystem<A, B> where T: Entity, IUIFormOnOpen<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnOpenSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnOpenSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnOpen((T)o, a, b);
        }

        protected abstract void UIFormOnOpen(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnOpenSystem<T, A, B, C> : SystemObject, IUIFormOnOpenSystem<A, B, C> where T: Entity, IUIFormOnOpen<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnOpenSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnOpenSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnOpen((T)o, a, b, c);
        }

        protected abstract void UIFormOnOpen(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnOpenSystem<T, A, B, C, D> : SystemObject, IUIFormOnOpenSystem<A, B, C, D> where T: Entity, IUIFormOnOpen<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnOpenSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnOpenSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnOpen((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnOpen(T self, A a, B b, C c, D d);
    }
}