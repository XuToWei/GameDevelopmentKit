namespace ET
{
    [ChildOf(typeof(SurvivorWorldComponent))]
    public sealed partial class SurvivorPlayerStateReactiveObserver: Entity, IAwake<SurvivorPlayerState>, IUpdate, IDestroy, IETReactive
    {
        public SurvivorPlayerState State { get; set; }

        [ETReactiveSource]
        public int Hp => this.State.Hp;

        [ETReactiveSource]
        public int Experience => this.State.Experience;
    }

    [ChildOf(typeof(SurvivorWorldComponent))]
    public sealed partial class SurvivorMonsterStateReactiveObserver: Entity, IAwake<SurvivorMonsterState>, IUpdate, IDestroy, IETReactive
    {
        public SurvivorMonsterState State { get; set; }

        [ETReactiveSource]
        public int Hp => this.State.Hp;
    }
}
