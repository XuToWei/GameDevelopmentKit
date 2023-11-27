using System;
using System.Diagnostics;
using ET;

namespace Bright.Serialization
{
    public static class TablesMemory
    {
        [StaticField]
        public static long MemorySize { get; private set; } = 0;

        [StaticField]
        private static long s_TotalMemorySize;

        [Conditional("UNITY_EDITOR")]
        public static void Clear()
        {
            MemorySize = 0;
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void BeginRecord()
        {
            s_TotalMemorySize = GC.GetTotalMemory(true);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void EndRecord()
        {
            MemorySize += Math.Min(0, GC.GetTotalMemory(true) - s_TotalMemorySize);
        }
    }
}