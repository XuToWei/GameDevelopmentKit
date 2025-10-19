using System;

namespace ET
{
    public interface IUIFormOnRecycle
    {
    }

    public interface IUIFormOnRecycle<A>
    {
    }

    public interface IUIFormOnRecycle<A, B>
    {
    }

    public interface IUIFormOnRecycle<A, B, C>
    {
    }

    public interface IUIFormOnRecycle<A, B, C, D>
    {
    }

    public interface IUIFormOnRecycleSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnRecycleSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnRecycleSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnRecycleSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnRecycleSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnRecycleSystem<T> : SystemObject, IUIFormOnRecycleSystem where T: Entity, IUIFormOnRecycle
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRecycleSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRecycleSystem.Run(Entity o)
        {
            this.UIFormOnRecycle((T)o);
        }

        protected abstract void UIFormOnRecycle(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnRecycleSystem<T, A> : SystemObject, IUIFormOnRecycleSystem<A> where T: Entity, IUIFormOnRecycle<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRecycleSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRecycleSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnRecycle((T)o, a);
        }

        protected abstract void UIFormOnRecycle(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnRecycleSystem<T, A, B> : SystemObject, IUIFormOnRecycleSystem<A, B> where T: Entity, IUIFormOnRecycle<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRecycleSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRecycleSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnRecycle((T)o, a, b);
        }

        protected abstract void UIFormOnRecycle(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnRecycleSystem<T, A, B, C> : SystemObject, IUIFormOnRecycleSystem<A, B, C> where T: Entity, IUIFormOnRecycle<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRecycleSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRecycleSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnRecycle((T)o, a, b, c);
        }

        protected abstract void UIFormOnRecycle(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnRecycleSystem<T, A, B, C, D> : SystemObject, IUIFormOnRecycleSystem<A, B, C, D> where T: Entity, IUIFormOnRecycle<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnRecycleSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnRecycleSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnRecycle((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnRecycle(T self, A a, B b, C c, D d);
    }
}