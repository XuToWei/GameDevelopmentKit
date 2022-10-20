namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIMatchRoomComponent: Entity, IAwake<UIMatchRoomView>, IDestroy
    {
        public UIMatchRoomView View { get; set; }
    }
}
