using System;

namespace ET
{
    public interface IUIFormOnUpdate
    {
    }

    public interface IUIFormOnUpdate<A>
    {
    }

    public interface IUIFormOnUpdate<A, B>
    {
    }

    public interface IUIFormOnUpdate<A, B, C>
    {
    }

    public interface IUIFormOnUpdate<A, B, C, D>
    {
    }

    public interface IUIFormOnUpdateSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnUpdateSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnUpdateSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnUpdateSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnUpdateSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnUpdateSystem<T> : SystemObject, IUIFormOnUpdateSystem where T: Entity, IUIFormOnUpdate
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnUpdateSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnUpdateSystem.Run(Entity o)
        {
            this.UIFormOnUpdate((T)o);
        }

        protected abstract void UIFormOnUpdate(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnUpdateSystem<T, A> : SystemObject, IUIFormOnUpdateSystem<A> where T: Entity, IUIFormOnUpdate<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnUpdateSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnUpdateSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnUpdate((T)o, a);
        }

        protected abstract void UIFormOnUpdate(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnUpdateSystem<T, A, B> : SystemObject, IUIFormOnUpdateSystem<A, B> where T: Entity, IUIFormOnUpdate<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnUpdateSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnUpdateSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnUpdate((T)o, a, b);
        }

        protected abstract void UIFormOnUpdate(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnUpdateSystem<T, A, B, C> : SystemObject, IUIFormOnUpdateSystem<A, B, C> where T: Entity, IUIFormOnUpdate<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnUpdateSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnUpdateSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnUpdate((T)o, a, b, c);
        }

        protected abstract void UIFormOnUpdate(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnUpdateSystem<T, A, B, C, D> : SystemObject, IUIFormOnUpdateSystem<A, B, C, D> where T: Entity, IUIFormOnUpdate<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnUpdateSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnUpdateSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnUpdate((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnUpdate(T self, A a, B b, C c, D d);
    }
}