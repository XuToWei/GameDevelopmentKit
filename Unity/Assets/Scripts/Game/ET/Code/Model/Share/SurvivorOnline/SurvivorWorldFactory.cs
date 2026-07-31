using ReactiveBinding;

namespace ET
{
    public static class SurvivorWorldFactory
    {
        public static SurvivorWorldData CreateWorld(string roomCode)
        {
            return new SurvivorWorldData
            {
                ServerTick = 0,
                NextStateId = 1,
                HostPlayerId = 0,
                RandomState = roomCode.GetHashCode(),
                RoomCode = roomCode,
                Phase = SurvivorRoomPhase.Lobby,
                Players = new VersionSyncDictionary<long, SurvivorPlayerState>(),
                Monsters = new VersionSyncDictionary<long, SurvivorMonsterState>(),
                Projectiles = new VersionSyncDictionary<long, SurvivorProjectileState>(),
                Pickups = new VersionSyncDictionary<long, SurvivorPickupState>(),
            };
        }

        public static SurvivorPlayerState CreatePlayer(long stateId, long playerId, string displayName)
        {
            return new SurvivorPlayerState
            {
                StateId = stateId,
                PlayerId = playerId,
                LastInputSequence = 0,
                DisplayName = displayName,
                PositionX = 0,
                PositionY = 0,
                MoveX = 0,
                MoveY = 0,
                Hp = SurvivorDefaults.PlayerMaxHp,
                MaxHp = SurvivorDefaults.PlayerMaxHp,
                Level = 1,
                Experience = 0,
                AutoFireCooldown = SurvivorDefaults.AutoFireIntervalTicks,
                Alive = true,
            };
        }

        public static SurvivorMonsterState CreateMonster(long stateId, int positionX, int positionY)
        {
            return new SurvivorMonsterState
            {
                StateId = stateId,
                TargetPlayerId = 0,
                ConfigId = 1,
                PositionX = positionX,
                PositionY = positionY,
                Hp = SurvivorDefaults.MonsterMaxHp,
                MaxHp = SurvivorDefaults.MonsterMaxHp,
                Alive = true,
            };
        }

        public static SurvivorProjectileState CreateProjectile(
            long stateId,
            long ownerPlayerId,
            int positionX,
            int positionY,
            int velocityX,
            int velocityY)
        {
            return new SurvivorProjectileState
            {
                StateId = stateId,
                OwnerPlayerId = ownerPlayerId,
                PositionX = positionX,
                PositionY = positionY,
                VelocityX = velocityX,
                VelocityY = velocityY,
                Damage = SurvivorDefaults.ProjectileDamage,
                RemainingTicks = SurvivorDefaults.ProjectileLifetimeTicks,
            };
        }

        public static SurvivorPickupState CreatePickup(long stateId, int positionX, int positionY, int experience)
        {
            return new SurvivorPickupState
            {
                StateId = stateId,
                PositionX = positionX,
                PositionY = positionY,
                Experience = experience,
            };
        }
    }
}
