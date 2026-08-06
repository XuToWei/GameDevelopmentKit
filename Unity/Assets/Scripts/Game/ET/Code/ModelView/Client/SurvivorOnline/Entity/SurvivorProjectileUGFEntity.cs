namespace ET.Client
{
    [ComponentOf(typeof(SurvivorProjectileEntry))]
    public sealed partial class SurvivorProjectileUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactive
    {
        public SurvivorProjectileState State { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        [ETReactiveSource]
        public int PositionX => this.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.State.PositionY;
    }
}
