using System;

namespace ET
{
    /// <summary>
    /// Declares a fieldless reactive binding group implemented by a Hotfix system.
    /// Binding state must be owned by the stable target type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ETReactiveSystemOfAttribute: BaseAttribute
    {
        public Type Type { get; }

        public string StateMemberName { get; }

        public ETReactiveSystemOfAttribute(Type type, string stateMemberName)
        {
            this.Type = type;
            this.StateMemberName = stateMemberName;
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
