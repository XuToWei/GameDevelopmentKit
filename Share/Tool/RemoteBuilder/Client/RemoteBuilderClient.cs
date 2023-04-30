namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class RemoteBuilderClient : Entity, IAwake, IDestroy
    {
        public Session Session;
    }
}