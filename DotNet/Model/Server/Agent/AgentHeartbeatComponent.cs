namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class AgentHeartbeatComponent : Entity, IAwake, IDestroy
    {
        public long Timer;
    }
}
