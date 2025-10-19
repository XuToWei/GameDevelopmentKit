using System;

namespace ET
{
    public interface IUIFormOnCover
    {
    }

    public interface IUIFormOnCover<A>
    {
    }

    public interface IUIFormOnCover<A, B>
    {
    }

    public interface IUIFormOnCover<A, B, C>
    {
    }

    public interface IUIFormOnCover<A, B, C, D>
    {
    }

    public interface IUIFormOnCoverSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnCoverSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnCoverSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnCoverSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnCoverSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnCoverSystem<T> : SystemObject, IUIFormOnCoverSystem where T: Entity, IUIFormOnCover
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCoverSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCoverSystem.Run(Entity o)
        {
            this.UIFormOnCover((T)o);
        }

        protected abstract void UIFormOnCover(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnCoverSystem<T, A> : SystemObject, IUIFormOnCoverSystem<A> where T: Entity, IUIFormOnCover<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCoverSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCoverSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnCover((T)o, a);
        }

        protected abstract void UIFormOnCover(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnCoverSystem<T, A, B> : SystemObject, IUIFormOnCoverSystem<A, B> where T: Entity, IUIFormOnCover<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCoverSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCoverSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnCover((T)o, a, b);
        }

        protected abstract void UIFormOnCover(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnCoverSystem<T, A, B, C> : SystemObject, IUIFormOnCoverSystem<A, B, C> where T: Entity, IUIFormOnCover<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCoverSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCoverSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnCover((T)o, a, b, c);
        }

        protected abstract void UIFormOnCover(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnCoverSystem<T, A, B, C, D> : SystemObject, IUIFormOnCoverSystem<A, B, C, D> where T: Entity, IUIFormOnCover<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnCoverSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnCoverSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnCover((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnCover(T self, A a, B b, C c, D d);
    }
}