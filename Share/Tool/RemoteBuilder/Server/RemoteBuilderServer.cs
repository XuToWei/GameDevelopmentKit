namespace ET
{
    [ChildOf(typeof(Scene))]
    public class RemoteBuilderServer : Entity, IAwake, IDestroy
    {
        public string BuildingVersion;

        public bool IsBuilding;
    }
}