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
                self.Runtime.Player = playerEnumerator.Current.Value;
                if (!self.Runtime.Player.Alive)
                {
                    continue;
                }

                self.TickSwordWave();
                self.TickAutoFire();
            }

            self.Runtime.Player = null;
            self.Runtime.TargetMonster = null;
        }

        private static void TickSwordWave(this SurvivorWorldComponent self)
        {
            if (self.Runtime.Player.SwordWaveCooldown > 0)
            {
                self.Runtime.Player.SwordWaveCooldown--;
            }

            if (self.Runtime.Player.SwordWaveCooldown > 0)
            {
                return;
            }

            self.Runtime.SwordWaveHitStateIds.Clear();
            using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (monsterEnumerator.MoveNext())
            {
                self.Runtime.Monster = monsterEnumerator.Current.Value;
                if (!self.Runtime.Monster.Alive)
                {
                    continue;
                }

                if (SurvivorMath.Abs(
                        self.Runtime.Monster.PositionX - self.Runtime.Player.PositionX) >
                    SurvivorDefaults.SwordWaveRangeX + SurvivorDefaults.MonsterCollisionRadius)
                {
                    continue;
                }

                if (SurvivorMath.Abs(
                        self.Runtime.Monster.PositionY - self.Runtime.Player.PositionY) >
                    SurvivorDefaults.SwordWaveRangeY + SurvivorDefaults.MonsterCollisionRadius)
                {
                    continue;
                }

                self.Runtime.SwordWaveHitStateIds.Add(self.Runtime.Monster.StateId);
            }

            self.Runtime.Monster = null;
            self.Runtime.Index = 0;
            while (self.Runtime.Index < self.Runtime.SwordWaveHitStateIds.Count)
            {
                self.Runtime.StateId = self.Runtime.SwordWaveHitStateIds[self.Runtime.Index];
                if (self.Data.Monsters.ContainsKey(self.Runtime.StateId))
                {
                    self.Runtime.Monster = self.Data.Monsters[self.Runtime.StateId];
                    self.Runtime.Monster.Hp -= SurvivorDefaults.SwordWaveDamage;
                }

                self.Runtime.Index++;
            }

            self.Runtime.Monster = null;
            self.Runtime.Player.SwordWaveRevision++;
            self.Runtime.Player.SwordWaveCooldown = SurvivorDefaults.SwordWaveIntervalTicks;
        }

        private static void TickAutoFire(this SurvivorWorldComponent self)
        {
            if (self.Runtime.Player.AutoFireLevel <= 0)
            {
                return;
            }

            if (self.Runtime.Player.AutoFireCooldown > 0)
            {
                self.Runtime.Player.AutoFireCooldown--;
            }

            if (self.Runtime.Player.AutoFireCooldown > 0)
            {
                return;
            }

            self.FindNearestMonster();
            if (self.Runtime.TargetMonster == null)
            {
                return;
            }

            self.Runtime.DeltaX =
                    self.Runtime.TargetMonster.PositionX - self.Runtime.Player.PositionX;
            self.Runtime.DeltaY =
                    self.Runtime.TargetMonster.PositionY - self.Runtime.Player.PositionY;
            self.CalculateAutoFireVelocity();
            self.SpawnProjectile(
                self.Runtime.VelocityX,
                self.Runtime.VelocityY);
            self.Runtime.Player.AutoFireCooldown = self.Runtime.Player.AutoFireIntervalTicks();
        }

        private static void FindNearestMonster(this SurvivorWorldComponent self)
        {
            self.Runtime.TargetMonster = null;
            self.Runtime.DistanceSquared = long.MaxValue;
            using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (monsterEnumerator.MoveNext())
            {
                self.Runtime.Monster = monsterEnumerator.Current.Value;
                if (!self.Runtime.Monster.Alive)
                {
                    continue;
                }

                long deltaX = self.Runtime.Monster.PositionX - self.Runtime.Player.PositionX;
                long deltaY = self.Runtime.Monster.PositionY - self.Runtime.Player.PositionY;
                long distanceSquared = deltaX * deltaX + deltaY * deltaY;
                if (distanceSquared > self.Runtime.DistanceSquared ||
                    distanceSquared == self.Runtime.DistanceSquared &&
                    self.Runtime.TargetMonster != null &&
                    self.Runtime.Monster.StateId >= self.Runtime.TargetMonster.StateId)
                {
                    continue;
                }

                self.Runtime.DistanceSquared = distanceSquared;
                self.Runtime.TargetMonster = self.Runtime.Monster;
            }

            self.Runtime.Monster = null;
        }

        private static void CalculateAutoFireVelocity(this SurvivorWorldComponent self)
        {
            if (self.Runtime.DeltaX == 0 && self.Runtime.DeltaY == 0)
            {
                self.Runtime.VelocityX = SurvivorDefaults.ProjectileMovePerTick;
                self.Runtime.VelocityY = 0;
                return;
            }

            double distance = Math.Sqrt(
                (long)self.Runtime.DeltaX * self.Runtime.DeltaX +
                (long)self.Runtime.DeltaY * self.Runtime.DeltaY);
            self.Runtime.VelocityX = (int)Math.Round(
                self.Runtime.DeltaX * SurvivorDefaults.ProjectileMovePerTick / distance,
                MidpointRounding.AwayFromZero);
            self.Runtime.VelocityY = (int)Math.Round(
                self.Runtime.DeltaY * SurvivorDefaults.ProjectileMovePerTick / distance,
                MidpointRounding.AwayFromZero);
        }

        private static void SpawnProjectile(
            this SurvivorWorldComponent self,
            int velocityX,
            int velocityY)
        {
            self.Runtime.StateId = self.AllocateStateId();
            self.Data.Projectiles.Add(
                self.Runtime.StateId,
                SurvivorWorldFactory.CreateProjectile(
                    self.Runtime.StateId,
                    self.Runtime.Player.PlayerId,
                    self.Runtime.Player.PositionX,
                    self.Runtime.Player.PositionY,
                    velocityX,
                    velocityY,
                    self.Runtime.Player.ProjectileDamage()));
            self.Data.ProjectileSetRevision++;
        }
    }
}
