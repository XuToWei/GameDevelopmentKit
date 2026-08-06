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

        public SurvivorPlayerEntry PlayerEntry { get; set; }

        public SurvivorMonsterEntry MonsterEntry { get; set; }

        public UGFEntity OwnerEntity { get; set; }

        public float VerticalOffset { get; set; }

        [ETReactiveSource]
        public int Hp => this.IsPlayer ? this.PlayerEntry.State.Hp : this.MonsterEntry.State.Hp;

        [ETReactiveSource]
        public int MaxHp => this.IsPlayer ? this.PlayerEntry.State.MaxHp : this.MonsterEntry.State.MaxHp;
    }
}
