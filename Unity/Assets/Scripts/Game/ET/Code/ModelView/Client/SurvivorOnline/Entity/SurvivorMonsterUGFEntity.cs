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
        public SurvivorMonsterEntry Entry { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        [ETReactiveSource]
        public int PositionX => this.Entry.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.Entry.State.PositionY;
    }
}
