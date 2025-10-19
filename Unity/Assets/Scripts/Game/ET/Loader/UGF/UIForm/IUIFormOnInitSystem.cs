using System;

namespace ET
{
    public interface IUIFormOnInit
    {
    }

    public interface IUIFormOnInit<A>
    {
    }

    public interface IUIFormOnInit<A, B>
    {
    }

    public interface IUIFormOnInit<A, B, C>
    {
    }

    public interface IUIFormOnInit<A, B, C, D>
    {
    }

    public interface IUIFormOnInitSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnInitSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnInitSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnInitSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnInitSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnInitSystem<T> : SystemObject, IUIFormOnInitSystem where T: Entity, IUIFormOnInit
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnInitSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnInitSystem.Run(Entity o)
        {
            this.UIFormOnInit((T)o);
        }

        protected abstract void UIFormOnInit(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnInitSystem<T, A> : SystemObject, IUIFormOnInitSystem<A> where T: Entity, IUIFormOnInit<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnInitSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnInitSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnInit((T)o, a);
        }

        protected abstract void UIFormOnInit(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnInitSystem<T, A, B> : SystemObject, IUIFormOnInitSystem<A, B> where T: Entity, IUIFormOnInit<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnInitSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnInitSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnInit((T)o, a, b);
        }

        protected abstract void UIFormOnInit(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnInitSystem<T, A, B, C> : SystemObject, IUIFormOnInitSystem<A, B, C> where T: Entity, IUIFormOnInit<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnInitSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnInitSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnInit((T)o, a, b, c);
        }

        protected abstract void UIFormOnInit(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnInitSystem<T, A, B, C, D> : SystemObject, IUIFormOnInitSystem<A, B, C, D> where T: Entity, IUIFormOnInit<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnInitSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnInitSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnInit((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnInit(T self, A a, B b, C c, D d);
    }
}