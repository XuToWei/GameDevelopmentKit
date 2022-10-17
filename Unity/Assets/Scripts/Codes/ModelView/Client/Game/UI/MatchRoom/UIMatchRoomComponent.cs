namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIMatchRoomComponent: Entity, IAwake<MatchRoomForm>, IDestroy
    {
        public MatchRoomForm Form { get; set; }
    }
}
