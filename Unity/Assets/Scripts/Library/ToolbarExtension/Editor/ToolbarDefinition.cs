using System;

namespace ToolbarExtension
{
    public enum OnGUISide : byte
    {
        Left,
        Right,
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ToolbarAttribute : Attribute
    {
        public OnGUISide side { get; }
        public int priority { get; }

        public ToolbarAttribute(OnGUISide side, int priority)
        {
            this.side = side;
            this.priority = priority;
        }
    }
}