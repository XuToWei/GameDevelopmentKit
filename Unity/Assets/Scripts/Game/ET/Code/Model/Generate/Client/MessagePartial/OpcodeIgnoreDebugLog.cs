using System.Collections.Generic;

namespace ET
{
    public class OpcodeIgnoreDebugLog : IOpcodeIgnoreDebugLog
    {
        public HashSet<ushort> IgnoreDebugLogMessageSet { get; } = new HashSet<ushort>()
        {
            OuterMessage.C2G_Ping,
            OuterMessage.G2C_Ping,
            OuterMessage.C2G_Benchmark,
            OuterMessage.G2C_Benchmark,
            ushort.MaxValue, // ActorResponse
        };
    }
}
