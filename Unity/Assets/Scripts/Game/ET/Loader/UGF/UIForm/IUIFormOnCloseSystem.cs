using System;

namespace ET
{
    public interface IUIFormOnClose
    {
    }

    public interface IUIFormOnClose<A>
    {
    }

    public interface IUIFormOnClose<A, B>
    {
    }

    public interface IUIFormOnClose<A, B, C>
    {
    }

    public interface IUIFormOnClose<A, B, C, D>
    {
    }

    public interface IUIFormOnCloseSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnCloseSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnCloseSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnCloseSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnCloseSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnCloseSystem<T> : SystemObject, IUIFormOnCloseSystem where T: Entity, IUIFormOnClose
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCloseSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCloseSystem.Run(Entity o)
        {
            this.UIFormOnClose((T)o);
        }

        protected abstract void UIFormOnClose(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnCloseSystem<T, A> : SystemObject, IUIFormOnCloseSystem<A> where T: Entity, IUIFormOnClose<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCloseSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCloseSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnClose((T)o, a);
        }

        protected abstract void UIFormOnClose(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnCloseSystem<T, A, B> : SystemObject, IUIFormOnCloseSystem<A, B> where T: Entity, IUIFormOnClose<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCloseSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCloseSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnClose((T)o, a, b);
        }

        protected abstract void UIFormOnClose(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnCloseSystem<T, A, B, C> : SystemObject, IUIFormOnCloseSystem<A, B, C> where T: Entity, IUIFormOnClose<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCloseSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCloseSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnClose((T)o, a, b, c);
        }

        protected abstract void UIFormOnClose(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnCloseSystem<T, A, B, C, D> : SystemObject, IUIFormOnCloseSystem<A, B, C, D> where T: Entity, IUIFormOnClose<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCloseSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCloseSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnClose((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnClose(T self, A a, B b, C c, D d);
    }
}