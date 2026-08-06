namespace ET
{
    public static class SurvivorProjectileSystem
    {
        public static void TickProjectiles(this SurvivorWorldComponent self)
        {
            self.Runtime.ProjectileRemovalStateIds.Clear();
            self.Runtime.ProjectileEnumerator = self.Data.Projectiles.GetEnumerator();
            while (self.Runtime.ProjectileEnumerator.MoveNext())
            {
                self.Runtime.Projectile = self.Runtime.ProjectileEnumerator.Current.Value;
                self.Runtime.Projectile.PositionX += self.Runtime.Projectile.VelocityX;
                self.Runtime.Projectile.PositionY += self.Runtime.Projectile.VelocityY;
                self.Runtime.Projectile.RemainingTicks--;
                if (self.Runtime.Projectile.RemainingTicks <= 0)
                {
                    self.Runtime.ProjectileRemovalStateIds.Add(self.Runtime.Projectile.StateId);
                    continue;
                }

                self.Runtime.Hit = false;
                self.Runtime.MonsterEnumerator = self.Data.Monsters.GetEnumerator();
                while (self.Runtime.MonsterEnumerator.MoveNext())
                {
                    self.Runtime.Monster = self.Runtime.MonsterEnumerator.Current.Value;
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

                self.Runtime.MonsterEnumerator.Dispose();
                self.Runtime.MonsterEnumerator = null;
                if (self.Runtime.Hit)
                {
                    self.Runtime.ProjectileRemovalStateIds.Add(self.Runtime.Projectile.StateId);
                }
            }

            self.Runtime.ProjectileEnumerator.Dispose();
            self.Runtime.ProjectileEnumerator = null;
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
