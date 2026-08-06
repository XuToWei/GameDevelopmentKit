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
        public SurvivorProjectileEntry Entry { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        [ETReactiveSource]
        public int PositionX => this.Entry.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.Entry.State.PositionY;
    }
}
