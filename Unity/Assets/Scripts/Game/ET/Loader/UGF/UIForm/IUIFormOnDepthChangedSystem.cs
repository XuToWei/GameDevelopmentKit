using System;

namespace ET
{
    public interface IUIFormOnDepthChanged
    {
    }

    public interface IUIFormOnDepthChanged<A>
    {
    }

    public interface IUIFormOnDepthChanged<A, B>
    {
    }

    public interface IUIFormOnDepthChanged<A, B, C>
    {
    }

    public interface IUIFormOnDepthChanged<A, B, C, D>
    {
    }

    public interface IUIFormOnDepthChangedSystem : ISystemType
    {
        void Run(Entity o);
    }

    public interface IUIFormOnDepthChangedSystem<A> : ISystemType
    {
        void Run(Entity o, A a);
    }

    public interface IUIFormOnDepthChangedSystem<A, B> : ISystemType
    {
        void Run(Entity o, A a, B b);
    }

    public interface IUIFormOnDepthChangedSystem<A, B, C> : ISystemType
    {
        void Run(Entity o, A a, B b, C c);
    }

    public interface IUIFormOnDepthChangedSystem<A, B, C, D> : ISystemType
    {
        void Run(Entity o, A a, B b, C c, D d);
    }

    [EntitySystem]
    public abstract class UIFormOnDepthChangedSystem<T> : SystemObject, IUIFormOnDepthChangedSystem where T: Entity, IUIFormOnDepthChanged
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnDepthChangedSystem);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnDepthChangedSystem.Run(Entity o)
        {
            this.UIFormOnDepthChanged((T)o);
        }

        protected abstract void UIFormOnDepthChanged(T self);
    }

    [EntitySystem]
    public abstract class UIFormOnDepthChangedSystem<T, A> : SystemObject, IUIFormOnDepthChangedSystem<A> where T: Entity, IUIFormOnDepthChanged<A>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnDepthChangedSystem<A>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnDepthChangedSystem<A>.Run(Entity o, A a)
        {
            this.UIFormOnDepthChanged((T)o, a);
        }

        protected abstract void UIFormOnDepthChanged(T self, A a);
    }

    [EntitySystem]
    public abstract class UIFormOnDepthChangedSystem<T, A, B> : SystemObject, IUIFormOnDepthChangedSystem<A, B> where T: Entity, IUIFormOnDepthChanged<A, B>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnDepthChangedSystem<A, B>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnDepthChangedSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.UIFormOnDepthChanged((T)o, a, b);
        }

        protected abstract void UIFormOnDepthChanged(T self, A a, B b);
    }

    [EntitySystem]
    public abstract class UIFormOnDepthChangedSystem<T, A, B, C> : SystemObject, IUIFormOnDepthChangedSystem<A, B, C> where T: Entity, IUIFormOnDepthChanged<A, B, C>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnDepthChangedSystem<A, B, C>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnDepthChangedSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.UIFormOnDepthChanged((T)o, a, b, c);
        }

        protected abstract void UIFormOnDepthChanged(T self, A a, B b, C c);
    }
    
    [EntitySystem]
    public abstract class UIFormOnDepthChangedSystem<T, A, B, C, D> : SystemObject, IUIFormOnDepthChangedSystem<A, B, C, D> where T: Entity, IUIFormOnDepthChanged<A, B, C, D>
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }

        Type ISystemType.SystemType()
        {
            return typeof(IUIFormOnDepthChangedSystem<A, B, C, D>);
        }

        int ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        void IUIFormOnDepthChangedSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.UIFormOnDepthChanged((T)o, a, b, c, d);
        }

        protected abstract void UIFormOnDepthChanged(T self, A a, B b, C c, D d);
    }
}