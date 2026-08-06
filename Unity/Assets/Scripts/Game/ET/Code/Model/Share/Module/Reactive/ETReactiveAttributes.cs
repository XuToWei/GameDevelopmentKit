using System;

namespace ET
{
    /// <summary>
    /// Generates reactive observation methods for an EntitySystemOf class.
    /// The EntitySystemOf owner must implement IETReactive.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ETReactiveSystemAttribute: BaseAttribute
    {
    }

    /// <summary>
    /// Marks a readable Entity property as a reactive source.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
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
