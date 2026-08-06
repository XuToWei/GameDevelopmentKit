namespace ET.Client
{
    [ComponentOf]
    public sealed partial class SurvivorHealthBarUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake<bool>,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactive
    {
        public bool IsPlayer { get; set; }

        public SurvivorPlayerState PlayerState { get; set; }

        public SurvivorMonsterState MonsterState { get; set; }

        public UGFEntity OwnerEntity { get; set; }

        public float VerticalOffset { get; set; }

        [ETReactiveSource]
        public int Hp => this.IsPlayer ? this.PlayerState.Hp : this.MonsterState.Hp;

        [ETReactiveSource]
        public int MaxHp => this.IsPlayer ? this.PlayerState.MaxHp : this.MonsterState.MaxHp;
    }
}
