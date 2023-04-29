using System.Collections.Generic;

namespace ET
{
    public interface IOpcodeIgnoreDebugLog
    {
        HashSet<ushort> IgnoreDebugLogMessageSet { get; }
    }
}
