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

        public SurvivorProjectilePrediction Prediction { get; set; }

        [ETReactiveSource]
        public int PositionX => this.Entry.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.Entry.State.PositionY;

        [ETReactiveSource]
        public int VelocityX => this.Entry.State.VelocityX;

        [ETReactiveSource]
        public int VelocityY => this.Entry.State.VelocityY;
    }
}
