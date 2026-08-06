namespace ET
{
    public static class SurvivorProjectileSystem
    {
        public static void TickProjectiles(this SurvivorWorldComponent self)
        {
            self.Runtime.ProjectileRemovalStateIds.Clear();
            using var projectileEnumerator = self.Data.Projectiles.GetEnumerator();
            while (projectileEnumerator.MoveNext())
            {
                self.Runtime.Projectile = projectileEnumerator.Current.Value;
                self.Runtime.Projectile.PositionX += self.Runtime.Projectile.VelocityX;
                self.Runtime.Projectile.PositionY += self.Runtime.Projectile.VelocityY;
                self.Runtime.Projectile.RemainingTicks--;
                if (self.Runtime.Projectile.RemainingTicks <= 0)
                {
                    self.Runtime.ProjectileRemovalStateIds.Add(self.Runtime.Projectile.StateId);
                    continue;
                }

                self.Runtime.Hit = false;
                using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
                while (monsterEnumerator.MoveNext())
                {
                    self.Runtime.Monster = monsterEnumerator.Current.Value;
                    if (!self.Runtime.Monster.Alive)
                    {
                        continue;
                    }

                    if (SurvivorMath.Abs(self.Runtime.Projectile.PositionX - self.Runtime.Monster.PositionX) >
                        SurvivorDefaults.ProjectileCollisionRadius + SurvivorDefaults.MonsterCollisionRadius)
                    {
                        continue;
                    }

                    if (SurvivorMath.Abs(self.Runtime.Projectile.PositionY - self.Runtime.Monster.PositionY) >
                        SurvivorDefaults.ProjectileCollisionRadius + SurvivorDefaults.MonsterCollisionRadius)
                    {
                        continue;
                    }

                    self.Runtime.Monster.Hp -= self.Runtime.Projectile.Damage;
                    self.Runtime.Hit = true;
                    break;
                }

                if (self.Runtime.Hit)
                {
                    self.Runtime.ProjectileRemovalStateIds.Add(self.Runtime.Projectile.StateId);
                }
            }

            self.Runtime.Index = 0;
            while (self.Runtime.Index < self.Runtime.ProjectileRemovalStateIds.Count)
            {
                self.Data.Projectiles.Remove(self.Runtime.ProjectileRemovalStateIds[self.Runtime.Index]);
                self.Data.ProjectileSetRevision++;
                self.Runtime.Index++;
            }

            self.Runtime.Projectile = null;
            self.Runtime.Monster = null;
        }
    }
}
