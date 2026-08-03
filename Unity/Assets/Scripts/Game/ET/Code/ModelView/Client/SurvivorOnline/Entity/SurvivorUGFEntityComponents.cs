namespace ET.Client
{
    [ComponentOf(typeof(SurvivorPlayerEntry))]
    public sealed partial class SurvivorPlayerUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactiveHost
    {
        public SurvivorPlayerState State { get; set; }
    }

    [ComponentOf(typeof(SurvivorMonsterEntry))]
    public sealed partial class SurvivorMonsterUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactiveHost
    {
        public SurvivorMonsterState State { get; set; }
    }

    [ComponentOf(typeof(SurvivorProjectileEntry))]
    public sealed partial class SurvivorProjectileUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactiveHost
    {
        public SurvivorProjectileState State { get; set; }
    }

    [ComponentOf(typeof(SurvivorPickupEntry))]
    public sealed partial class SurvivorPickupUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactiveHost
    {
        public SurvivorPickupState State { get; set; }
    }
}
