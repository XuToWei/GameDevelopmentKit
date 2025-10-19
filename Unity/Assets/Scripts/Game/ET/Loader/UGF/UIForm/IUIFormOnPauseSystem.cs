using System;

namespace ET
{
    public interface IUIFormOnPause
    {
    }

    public interface IUIFormOnPause<A>
    {
    }

    public interface IUIFormOnPause<A, B>
    {
    }

    public interface IUIFormOnPause<A, B, C>
    {
    }

    public interface IUIFormOnPause<A, B, C, D>
    {
    }

    public interface IUIFormOnPauseSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnPauseSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnPauseSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnPauseSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnPauseSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnPauseSystem<T> : SystemObject, IUIFormOnPauseSystem where T: Entity, IUIFormOnPause
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnPauseSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnPauseSystem.Run(Entity o)
        {
            this.UIFormOnPause((T)o);
        }

        protected abstract void UIFormOnPause(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnPauseSystem<T, A> : SystemObject, IUIFormOnPauseSystem<A> where T: Entity, IUIFormOnPause<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnPauseSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnPauseSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnPause((T)o, a);
        }

        protected abstract void UIFormOnPause(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnPauseSystem<T, A, B> : SystemObject, IUIFormOnPauseSystem<A, B> where T: Entity, IUIFormOnPause<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnPauseSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnPauseSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnPause((T)o, a, b);
        }

        protected abstract void UIFormOnPause(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnPauseSystem<T, A, B, C> : SystemObject, IUIFormOnPauseSystem<A, B, C> where T: Entity, IUIFormOnPause<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnPauseSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnPauseSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnPause((T)o, a, b, c);
        }

        protected abstract void UIFormOnPause(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnPauseSystem<T, A, B, C, D> : SystemObject, IUIFormOnPauseSystem<A, B, C, D> where T: Entity, IUIFormOnPause<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnPauseSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnPauseSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnPause((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnPause(T self, A a, B b, C c, D d);
    }
}