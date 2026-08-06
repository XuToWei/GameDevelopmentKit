namespace ET
{
    public enum SurvivorWorldRole: byte
    {
        ServerAuthority = 1,
        SnapshotConsumer = 2,
    }

    public enum SurvivorRoomPhase: byte
    {
        Lobby = 1,
        Running = 2,
        Ended = 3,
    }

    public enum SurvivorSkillType: byte
    {
        None = 0,
        AutoFire = 1,
        PowerShot = 2,
        SwiftStep = 3,
    }

    public static class SurvivorDefaults
    {
        public const int MaxPlayers = 4;
        public const int SimulationTicksPerSecond = 20;
        public const int SnapshotTicks = 2;
        public const int FullSnapshotInterval = 50;
        public const int InputScale = 1000;
        public const int ArenaHalfExtent = 10000;
        public const int PlayerMovePerTick = 180;
        public const int PlayerMaxHp = 100;
        public const int PlayerCollisionRadius = 450;
        public const int MonsterMovePerTick = 70;
        public const int MonsterMaxHp = 30;
        public const int MonsterCollisionRadius = 400;
        public const int ProjectileMovePerTick = 450;
        public const int ProjectileDamage = 10;
        public const int ProjectileLifetimeTicks = 30;
        public const int ProjectileCollisionRadius = 180;
        public const int ExperiencePickupRange = 1500;
        public const int SwordWaveIntervalTicks = 20;
        public const int SwordWaveDamage = 10;
        public const int SwordWaveRangeX = 3000;
        public const int SwordWaveRangeY = 750;
        public const int AutoFireIntervalTicks = 10;
        public const int MonsterSpawnIntervalTicks = 20;
        public const int MaxMonsters = 64;
        public const int MonsterContactDamagePerTick = 1;
        public const int MonsterKillExperience = 5;
        public const int LevelExperienceStep = 10;
        public const int LevelMaxHpIncrease = 10;
        public const int SpawnDistance = 8000;
        public const int AutoFireIntervalReductionTicks = 2;
        public const int MinimumAutoFireIntervalTicks = 2;
        public const int PowerShotDamagePerLevel = 5;
        public const int SwiftStepMovePerTickPerLevel = 30;

        public static int ClampPlayerPosition(int position)
        {
            int limit = ArenaHalfExtent - PlayerCollisionRadius;
            if (position < -limit)
            {
                return -limit;
            }

            return position > limit ? limit : position;
        }

        public static float ClampPlayerPresentationPosition(float position)
        {
            float limit = (ArenaHalfExtent - PlayerCollisionRadius) / 1000f;
            if (position < -limit)
            {
                return -limit;
            }

            return position > limit ? limit : position;
        }
    }
}
