namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public sealed class SurvivorClientComponent: Entity, IAwake, IDestroy
    {
        public ClientSenderComponent ClientSender { get; set; }

        public EntityRef<SurvivorRoom> Room { get; set; }

        public EntityRef<SurvivorWorldComponent> World { get; set; }

        public SurvivorLocalPlayerPrediction LocalPrediction { get; set; }

        public long PlayerId { get; set; }

        public long LastSequence { get; set; }

        public long InputSequence { get; set; }

        public bool IsHost { get; set; }

        public bool HasBaseline { get; set; }

        public SurvivorWorldComponent WorldComponent => this.World;

        /// <summary>尚未收到首个完整快照或玩家还没进入房间时为 null，这是真实的动态状态而非架构缺失。</summary>
        public SurvivorPlayerState LocalPlayer
        {
            get
            {
                if (!this.HasBaseline)
                {
                    return null;
                }

                return this.WorldComponent.Data.Players.TryGetValue(this.PlayerId, out SurvivorPlayerState player) ? player : null;
            }
        }

        /// <summary>供 View 层观察的业务状态。没有基线时对外表现为 Lobby。</summary>
        public SurvivorRoomPhase Phase => this.HasBaseline ? this.WorldComponent.Data.Phase : SurvivorRoomPhase.Lobby;

        public bool SkillChoiceAvailable
        {
            get
            {
                SurvivorPlayerState player = this.LocalPlayer;
                return player != null && this.Phase == SurvivorRoomPhase.Running && player.UnspentSkillPoints > 0;
            }
        }
    }
}
