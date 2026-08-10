namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public sealed partial class UIFormSurvivorHudComponent:
            UGFUIForm<MonoUIFormSurvivorHud>,
            IAwake,
            IUGFUIFormOnUpdate,
            IUGFUIFormOnClose,
            IETReactive
    {
        public SurvivorClientComponent Client { get; set; }

        /// <summary>
        /// HUD 可能在世界被换掉（返回房间重新加入）之后还残留一帧观察，
        /// 因此所有 Source 都要经过 HasBaseline / LocalPlayer 的动态状态判断。
        /// 玩家数值用无参方法 Source，避免同一帧内重复查字典。
        /// </summary>
        [ETReactiveSource]
        public string RoomCode => this.Client.HasBaseline ? this.Client.WorldComponent.Data.RoomCode : string.Empty;

        [ETReactiveSource]
        public long ServerTick => this.Client.HasBaseline ? this.Client.WorldComponent.Data.ServerTick : 0;

        [ETReactiveSource]
        public SurvivorRoomPhase Phase => this.Client.Phase;

        [ETReactiveSource]
        public int Hp()
        {
            SurvivorPlayerState player = this.Client.LocalPlayer;
            return player == null ? 0 : player.Hp;
        }

        [ETReactiveSource]
        public int MaxHp()
        {
            SurvivorPlayerState player = this.Client.LocalPlayer;
            return player == null ? 0 : player.MaxHp;
        }

        [ETReactiveSource]
        public int Level()
        {
            SurvivorPlayerState player = this.Client.LocalPlayer;
            return player == null ? 0 : player.Level;
        }
    }
}
