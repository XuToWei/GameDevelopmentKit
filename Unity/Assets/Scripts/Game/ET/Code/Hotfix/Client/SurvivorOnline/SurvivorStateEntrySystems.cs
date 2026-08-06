namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorPlayerEntry))]
    public static partial class SurvivorPlayerEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerEntry self)
        {
        }
    }

    [EntitySystemOf(typeof(SurvivorMonsterEntry))]
    public static partial class SurvivorMonsterEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorMonsterEntry self)
        {
        }
    }

    [EntitySystemOf(typeof(SurvivorProjectileEntry))]
    public static partial class SurvivorProjectileEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorProjectileEntry self)
        {
        }
    }

    [EntitySystemOf(typeof(SurvivorPickupEntry))]
    public static partial class SurvivorPickupEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPickupEntry self)
        {
        }
    }
}
