using ReactiveBinding;

namespace ET.Client
{
    [ComponentOf(typeof(SurvivorPlayerEntry))]
    public sealed class SurvivorPlayerUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide
    {
        public IReactiveObserver Observer { get; set; }
    }

    [ComponentOf(typeof(SurvivorMonsterEntry))]
    public sealed class SurvivorMonsterUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnHide
    {
        public IReactiveObserver Observer { get; set; }
    }

    [ComponentOf(typeof(SurvivorProjectileEntry))]
    public sealed class SurvivorProjectileUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnHide
    {
        public IReactiveObserver Observer { get; set; }
    }

    [ComponentOf(typeof(SurvivorPickupEntry))]
    public sealed class SurvivorPickupUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnHide
    {
        public IReactiveObserver Observer { get; set; }
    }
}
