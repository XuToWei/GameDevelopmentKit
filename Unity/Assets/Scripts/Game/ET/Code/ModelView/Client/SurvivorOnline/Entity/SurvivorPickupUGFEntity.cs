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
        public SurvivorPickupState State { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        [ETReactiveSource]
        public int PositionX => this.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.State.PositionY;
    }
}
