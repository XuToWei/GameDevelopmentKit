namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorPlayerEntry))]
    public static partial class SurvivorPlayerEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerEntry self, SurvivorPlayerState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorPlayerEntry self)
        {
            self.State = null;
        }
    }

    [EntitySystemOf(typeof(SurvivorMonsterEntry))]
    public static partial class SurvivorMonsterEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorMonsterEntry self, SurvivorMonsterState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorMonsterEntry self)
        {
            self.State = null;
        }
    }

    [EntitySystemOf(typeof(SurvivorProjectileEntry))]
    public static partial class SurvivorProjectileEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorProjectileEntry self, SurvivorProjectileState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorProjectileEntry self)
        {
            self.State = null;
        }
    }

    [EntitySystemOf(typeof(SurvivorPickupEntry))]
    public static partial class SurvivorPickupEntrySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPickupEntry self, SurvivorPickupState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorPickupEntry self)
        {
            self.State = null;
        }
    }
}
