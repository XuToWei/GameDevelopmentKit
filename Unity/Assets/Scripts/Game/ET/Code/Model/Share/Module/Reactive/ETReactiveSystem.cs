using System;
using System.Collections.Generic;
using System.Reflection;
using ReactiveBinding;

namespace ET
{
    /// <summary>
    /// Stable Entity-side access to a generated Hotfix reactive observer.
    /// </summary>
    public interface IETReactiveHost
    {
        IReactiveObserver ReactiveObserver { get; set; }
    }

    /// <summary>
    /// Lifecycle contract implemented by Hotfix-generated reactive observers.
    /// </summary>
    public interface IETReactiveObserver: IReactiveObserver
    {
        int DllVersion { get; }

        long OwnerInstanceId { get; }

        void Initialize(IETReactiveHost host, int dllVersion);

        void Recycle();
    }

    /// <summary>
    /// Holds the generated observer Types for the currently loaded Hotfix DLLs.
    /// </summary>
    [Code]
    public sealed class ETReactiveSystem: Singleton<ETReactiveSystem>, ISingletonAwake
    {
        private const int ObserverPoolCapacity = 64;

        [StaticField]
        private static int currentDllVersion;

        private readonly Dictionary<Type, Type> observerTypes = new();

        private readonly Dictionary<Type, ObserverPool> observerPools = new();

        public int DllVersion { get; private set; }

        public void Awake()
        {
            this.observerTypes.Clear();
            this.observerPools.Clear();

            foreach (Type observerType in CodeTypes.Instance.GetTypes(typeof(ETReactiveObserverAttribute)))
            {
                var attribute = observerType.GetCustomAttribute<ETReactiveObserverAttribute>(false);
                if (attribute == null || observerType.IsAbstract || !typeof(IETReactiveObserver).IsAssignableFrom(observerType))
                {
                    throw new InvalidOperationException($"Invalid ET reactive observer Type: {observerType.FullName}");
                }

                if (!typeof(IETReactiveHost).IsAssignableFrom(attribute.OwnerType))
                {
                    throw new InvalidOperationException(
                        $"ET reactive host Type must implement {nameof(IETReactiveHost)}: {attribute.OwnerType.FullName}");
                }

                if (this.observerTypes.TryGetValue(attribute.OwnerType, out var registeredType))
                {
                    throw new InvalidOperationException(
                        $"Duplicate ET reactive observer for {attribute.OwnerType.FullName}: " +
                        $"{registeredType.FullName}, {observerType.FullName}");
                }

                this.observerTypes.Add(attribute.OwnerType, observerType);
                this.observerPools.Add(observerType, new ObserverPool(observerType));
            }

            this.DllVersion = ++currentDllVersion;
        }

        public Type GetObserverType(Type ownerType)
        {
            if (this.observerTypes.TryGetValue(ownerType, out var observerType))
            {
                return observerType;
            }

            throw new InvalidOperationException($"ET reactive observer Type not found for owner: {ownerType.FullName}");
        }

        public IETReactiveObserver Rent(Type ownerType, IETReactiveHost host)
        {
            if (ownerType == null)
            {
                throw new ArgumentNullException(nameof(ownerType));
            }

            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (!ownerType.IsInstanceOfType(host))
            {
                throw new ArgumentException(
                    $"Reactive host instance '{host.GetType().FullName}' is not assignable to '{ownerType.FullName}'.",
                    nameof(host));
            }

            Type observerType = this.GetObserverType(ownerType);
            IETReactiveObserver observer = this.observerPools[observerType].Rent();
            observer.Initialize(host, this.DllVersion);
            return observer;
        }

        public void Recycle(IETReactiveObserver observer)
        {
            if (observer == null)
            {
                return;
            }

            if (observer.DllVersion == this.DllVersion &&
                this.observerPools.TryGetValue(observer.GetType(), out var pool))
            {
                observer.Recycle();
                pool.Return(observer);
                return;
            }

            observer.Recycle();
        }

        [EnableClass]
        private sealed class ObserverPool
        {
            private readonly Type observerType;

            private readonly Stack<IETReactiveObserver> observers = new();

            public ObserverPool(Type observerType)
            {
                this.observerType = observerType;
            }

            public IETReactiveObserver Rent()
            {
                lock (this.observers)
                {
                    if (this.observers.Count > 0)
                    {
                        return this.observers.Pop();
                    }
                }

                return Activator.CreateInstance(this.observerType, true) as IETReactiveObserver ??
                        throw new InvalidOperationException($"Could not create ET reactive observer: {this.observerType.FullName}");
            }

            public void Return(IETReactiveObserver observer)
            {
                lock (this.observers)
                {
                    if (this.observers.Count < ObserverPoolCapacity)
                    {
                        this.observers.Push(observer);
                    }
                }
            }
        }
    }
}
