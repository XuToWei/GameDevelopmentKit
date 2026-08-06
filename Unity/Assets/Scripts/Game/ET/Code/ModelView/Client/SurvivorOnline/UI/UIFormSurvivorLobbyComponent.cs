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

        public SurvivorWorldComponent WorldComponent => this.Client.World;

        [ETReactiveSource]
        public string RoomCode => this.Client.HasBaseline ? this.WorldComponent.Data.RoomCode : string.Empty;

        [ETReactiveSource]
        public SurvivorRoomPhase Phase => this.Client.HasBaseline ? this.WorldComponent.Data.Phase : SurvivorRoomPhase.Lobby;

        [ETReactiveSource]
        public bool IsHost => this.Client.IsHost;
    }
}
