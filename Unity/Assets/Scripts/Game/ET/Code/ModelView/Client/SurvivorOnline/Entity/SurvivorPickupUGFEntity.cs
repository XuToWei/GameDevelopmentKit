namespace ET.Client
{
    [ComponentOf(typeof(SurvivorPickupEntry))]
    public sealed partial class SurvivorPickupUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactive
    {
        public SurvivorPickupEntry Entry { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        [ETReactiveSource]
        public int PositionX => this.Entry.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.Entry.State.PositionY;
    }
}
