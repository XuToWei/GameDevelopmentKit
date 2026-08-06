namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed partial class UIFormSurvivorHudComponent:
            UGFUIForm<MonoUIFormSurvivorHud>,
            IAwake,
            IUGFUIFormOnOpen,
            IUGFUIFormOnUpdate,
            IUGFUIFormOnClose,
            IETReactive
    {
        public SurvivorClientComponent Client { get; set; }

        public SurvivorWorldComponent WorldComponent => this.Client.World;

        public SurvivorWorldData WorldData => this.WorldComponent.Data;

        public SurvivorPlayerState LocalPlayerState => this.WorldData.Players[this.Client.PlayerId];

        [ETReactiveSource]
        public string RoomCode => this.WorldData.RoomCode;

        [ETReactiveSource]
        public long ServerTick => this.WorldData.ServerTick;

        [ETReactiveSource]
        public SurvivorRoomPhase Phase => this.WorldData.Phase;

        [ETReactiveSource]
        public int Hp => this.LocalPlayerState.Hp;

        [ETReactiveSource]
        public int MaxHp => this.LocalPlayerState.MaxHp;

        [ETReactiveSource]
        public int Level => this.LocalPlayerState.Level;
    }
}
