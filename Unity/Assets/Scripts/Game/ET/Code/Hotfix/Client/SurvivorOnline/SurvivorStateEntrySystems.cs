using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorPlayerEntry))]
    public static partial class SurvivorPlayerEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerEntry self)
        {
            EventSystem.Instance.PublishAsync(
                self.Root(),
                new SurvivorPlayerEntryCreated { Entry = self }).Forget();
        }
    }

    [EntitySystemOf(typeof(SurvivorMonsterEntry))]
    public static partial class SurvivorMonsterEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorMonsterEntry self)
        {
            EventSystem.Instance.PublishAsync(
                self.Root(),
                new SurvivorMonsterEntryCreated { Entry = self }).Forget();
        }
    }

    [EntitySystemOf(typeof(SurvivorProjectileEntry))]
    public static partial class SurvivorProjectileEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorProjectileEntry self)
        {
            EventSystem.Instance.PublishAsync(
                self.Root(),
                new SurvivorProjectileEntryCreated { Entry = self }).Forget();
        }
    }

    [EntitySystemOf(typeof(SurvivorPickupEntry))]
    public static partial class SurvivorPickupEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPickupEntry self)
        {
            EventSystem.Instance.PublishAsync(
                self.Root(),
                new SurvivorPickupEntryCreated { Entry = self }).Forget();
        }
    }
}
