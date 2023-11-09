using System.Diagnostics;

namespace CodeBind
{
    [Conditional("UNITY_EDITOR")]
    public sealed class MonoCodeBindAttribute : CodeBindAttribute
    {
        public readonly char separatorChar;

        public MonoCodeBindAttribute(char separatorChar)
        {
            this.separatorChar = separatorChar;
        }
        
        public MonoCodeBindAttribute()
        {
            this.separatorChar = '_';
        }
    }
}
