namespace ET.Client
{
    [ComponentOf(typeof(SurvivorDamageNumberEntry))]
    public sealed partial class SurvivorDamageNumberUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide
    {
        public float ElapsedSeconds { get; set; }
    }
}
