using System;

namespace ET
{
    /// <summary>
    /// Generates a Hotfix-local reactive observer for an EntitySystemOf class.
    /// The EntitySystemOf owner must implement IETReactiveHost.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ETReactiveSystemAttribute: BaseAttribute
    {
    }

    /// <summary>
    /// Associates a generated observer Type with its stable reactive owner Type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ETReactiveObserverAttribute: BaseAttribute
    {
        public Type OwnerType { get; }

        public ETReactiveObserverAttribute(Type ownerType)
        {
            this.OwnerType = ownerType;
        }
    }

    /// <summary>
    /// Marks a static method whose first argument is the reactive owner.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ETReactiveSourceAttribute: BaseAttribute
    {
    }

    /// <summary>
    /// Marks a static method to invoke when one or more reactive sources change.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ETReactiveBindAttribute: BaseAttribute
    {
        public string[] ReactiveIds { get; }

        public ETReactiveBindAttribute(params string[] reactiveIds)
        {
            this.ReactiveIds = reactiveIds;
        }
    }
}
