namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed partial class UIFormSurvivorLobbyComponent:
            UGFUIForm<MonoUIFormSurvivorLobby>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnUpdate,
            IUGFUIFormOnClose,
            IETReactive
    {
        public SurvivorClientComponent Client { get; set; }

        [ETReactiveSource]
        public string StatusMessage { get; set; }

        [ETReactiveSource]
        public string RoomCode => this.Client.HasBaseline ? this.Client.WorldComponent.Data.RoomCode : string.Empty;

        [ETReactiveSource]
        public SurvivorRoomPhase Phase => this.Client.Phase;

        [ETReactiveSource]
        public bool IsHost => this.Client.IsHost;
    }
}
