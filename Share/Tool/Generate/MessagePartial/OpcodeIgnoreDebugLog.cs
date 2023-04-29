using System.Collections.Generic;

namespace ET
{
    public class OpcodeIgnoreDebugLog : IOpcodeIgnoreDebugLog
    {
        public HashSet<ushort> IgnoreDebugLogMessageSet { get; } = new HashSet<ushort>()
        {
            ushort.MaxValue,
        };
    }
}
