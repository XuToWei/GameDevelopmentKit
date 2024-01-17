using System;
using System.Diagnostics;

namespace Luban
{
    public static class TablesMemory
    {
        public static long MemorySize { get; private set; } = 0;

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