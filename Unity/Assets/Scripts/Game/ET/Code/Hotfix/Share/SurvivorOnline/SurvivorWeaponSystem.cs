using System;

namespace ET
{
    public static class SurvivorWeaponSystem
    {
        public static void TickWeapons(this SurvivorWorldComponent self)
        {
            using var playerEnumerator = self.Data.Players.GetEnumerator();
            while (playerEnumerator.MoveNext())
            {
                SurvivorPlayerState player = playerEnumerator.Current.Value;
                if (!player.Alive)
                {
                    continue;
                }

                self.TickSwordWave(player);
                self.TickAutoFire(player);
            }
        }

        private static void TickSwordWave(this SurvivorWorldComponent self, SurvivorPlayerState player)
        {
            if (player.SwordWaveCooldown > 0)
            {
                player.SwordWaveCooldown--;
            }

            if (player.SwordWaveCooldown > 0)
            {
                return;
            }

            self.Runtime.SwordWaveHitStateIds.Clear();
            using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (monsterEnumerator.MoveNext())
            {
                SurvivorMonsterState monster = monsterEnumerator.Current.Value;
                if (!monster.Alive)
                {
                    continue;
                }

                if (SurvivorMath.Abs(monster.PositionX - player.PositionX) >
                    SurvivorDefaults.SwordWaveRangeX + SurvivorDefaults.MonsterCollisionRadius)
                {
                    continue;
                }

                if (SurvivorMath.Abs(monster.PositionY - player.PositionY) >
                    SurvivorDefaults.SwordWaveRangeY + SurvivorDefaults.MonsterCollisionRadius)
                {
                    continue;
                }

                self.Runtime.SwordWaveHitStateIds.Add(monster.StateId);
            }

            for (int index = 0; index < self.Runtime.SwordWaveHitStateIds.Count; index++)
            {
                long stateId = self.Runtime.SwordWaveHitStateIds[index];
                if (self.Data.Monsters.TryGetValue(stateId, out SurvivorMonsterState monster))
                {
                    monster.Hp -= SurvivorDefaults.SwordWaveDamage;
                }
            }

            player.SwordWaveRevision++;
            player.SwordWaveCooldown = SurvivorDefaults.SwordWaveIntervalTicks;
        }

        private static void TickAutoFire(this SurvivorWorldComponent self, SurvivorPlayerState player)
        {
            if (player.AutoFireLevel <= 0)
            {
                return;
            }

            if (player.AutoFireCooldown > 0)
            {
                player.AutoFireCooldown--;
            }

            if (player.AutoFireCooldown > 0)
            {
                return;
            }

            SurvivorMonsterState targetMonster = self.FindNearestMonster(player);
            if (targetMonster == null)
            {
                return;
            }

            int deltaX = targetMonster.PositionX - player.PositionX;
            int deltaY = targetMonster.PositionY - player.PositionY;
            CalculateAutoFireVelocity(deltaX, deltaY, out int velocityX, out int velocityY);
            self.SpawnProjectile(player, velocityX, velocityY);
            player.AutoFireCooldown = player.AutoFireIntervalTicks();
        }

        private static SurvivorMonsterState FindNearestMonster(this SurvivorWorldComponent self, SurvivorPlayerState player)
        {
            SurvivorMonsterState targetMonster = null;
            long nearestDistanceSquared = long.MaxValue;
            using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (monsterEnumerator.MoveNext())
            {
                SurvivorMonsterState monster = monsterEnumerator.Current.Value;
                if (!monster.Alive)
                {
                    continue;
                }

                long deltaX = monster.PositionX - player.PositionX;
                long deltaY = monster.PositionY - player.PositionY;
                long distanceSquared = deltaX * deltaX + deltaY * deltaY;
                if (distanceSquared > nearestDistanceSquared ||
                    distanceSquared == nearestDistanceSquared && targetMonster != null && monster.StateId >= targetMonster.StateId)
                {
                    continue;
                }

                nearestDistanceSquared = distanceSquared;
                targetMonster = monster;
            }

            return targetMonster;
        }

        private static void CalculateAutoFireVelocity(int deltaX, int deltaY, out int velocityX, out int velocityY)
        {
            if (deltaX == 0 && deltaY == 0)
            {
                velocityX = SurvivorDefaults.ProjectileMovePerTick;
                velocityY = 0;
                return;
            }

            double distance = Math.Sqrt((long)deltaX * deltaX + (long)deltaY * deltaY);
            velocityX = (int)Math.Round(deltaX * SurvivorDefaults.ProjectileMovePerTick / distance, MidpointRounding.AwayFromZero);
            velocityY = (int)Math.Round(deltaY * SurvivorDefaults.ProjectileMovePerTick / distance, MidpointRounding.AwayFromZero);
        }

        private static void SpawnProjectile(this SurvivorWorldComponent self, SurvivorPlayerState player, int velocityX, int velocityY)
        {
            long stateId = self.AllocateStateId();
            self.Data.Projectiles.Add(
                stateId,
                SurvivorWorldFactory.CreateProjectile(
                    stateId,
                    player.PlayerId,
                    player.PositionX,
                    player.PositionY,
                    velocityX,
                    velocityY,
                    player.ProjectileDamage()));
            self.Data.ProjectileSetRevision++;
        }
    }
}
