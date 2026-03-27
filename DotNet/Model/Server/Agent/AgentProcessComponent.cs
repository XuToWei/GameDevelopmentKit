using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class AgentProcessComponent : Entity, IAwake
    {
        public readonly Dictionary<int, System.Diagnostics.Process> Processes = new();
    }
}
