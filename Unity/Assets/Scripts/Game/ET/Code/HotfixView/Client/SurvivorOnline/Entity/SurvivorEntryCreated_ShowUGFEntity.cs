using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.SurvivorView)]
    public sealed class SurvivorPlayerEntryCreated_ShowUGFEntity:
            AEvent<Scene, SurvivorPlayerEntryCreated>
    {
        protected override async UniTask Run(Scene scene, SurvivorPlayerEntryCreated args)
        {
            args.Entry.AddComponent<SurvivorPlayerUGFEntity>();
            await args.Entry
                    .GetComponent<SurvivorPlayerUGFEntity>()
                    .ShowEntityAsync(UGFEntityId.SurvivorPlayer);
        }
    }

    [Event(SceneType.SurvivorView)]
    public sealed class SurvivorMonsterEntryCreated_ShowUGFEntity:
            AEvent<Scene, SurvivorMonsterEntryCreated>
    {
        protected override async UniTask Run(Scene scene, SurvivorMonsterEntryCreated args)
        {
            args.Entry.AddComponent<SurvivorMonsterUGFEntity>();
            await args.Entry
                    .GetComponent<SurvivorMonsterUGFEntity>()
                    .ShowEntityAsync(UGFEntityId.SurvivorMonster);
        }
    }

    [Event(SceneType.SurvivorView)]
    public sealed class SurvivorProjectileEntryCreated_ShowUGFEntity:
            AEvent<Scene, SurvivorProjectileEntryCreated>
    {
        protected override async UniTask Run(Scene scene, SurvivorProjectileEntryCreated args)
        {
            args.Entry.AddComponent<SurvivorProjectileUGFEntity>();
            await args.Entry
                    .GetComponent<SurvivorProjectileUGFEntity>()
                    .ShowEntityAsync(UGFEntityId.SurvivorProjectile);
        }
    }

    [Event(SceneType.SurvivorView)]
    public sealed class SurvivorPickupEntryCreated_ShowUGFEntity:
            AEvent<Scene, SurvivorPickupEntryCreated>
    {
        protected override async UniTask Run(Scene scene, SurvivorPickupEntryCreated args)
        {
            args.Entry.AddComponent<SurvivorPickupUGFEntity>();
            await args.Entry
                    .GetComponent<SurvivorPickupUGFEntity>()
                    .ShowEntityAsync(UGFEntityId.SurvivorPickup);
        }
    }
}
