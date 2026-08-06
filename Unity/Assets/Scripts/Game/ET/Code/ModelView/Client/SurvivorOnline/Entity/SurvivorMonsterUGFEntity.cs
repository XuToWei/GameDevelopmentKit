namespace ET.Client
{
    [ComponentOf(typeof(SurvivorMonsterEntry))]
    public sealed partial class SurvivorMonsterUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactive
    {
        public SurvivorMonsterState State { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        [ETReactiveSource]
        public int PositionX => this.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.State.PositionY;
    }
}
