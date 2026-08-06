namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public sealed partial class SurvivorClientComponent: Entity, IAwake, IUpdate, IDestroy, IETReactive
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

        [ETReactiveSource]
        public long SkillChoiceRevision => this.HasBaseline ? this.WorldComponent.Data.Players[this.PlayerId].SkillChoiceRevision : 0;

        [ETReactiveSource]
        public int UnspentSkillPoints => this.HasBaseline ? this.WorldComponent.Data.Players[this.PlayerId].UnspentSkillPoints : 0;

        [ETReactiveSource]
        public SurvivorRoomPhase Phase => this.HasBaseline ? this.WorldComponent.Data.Phase : SurvivorRoomPhase.Lobby;
    }
}
