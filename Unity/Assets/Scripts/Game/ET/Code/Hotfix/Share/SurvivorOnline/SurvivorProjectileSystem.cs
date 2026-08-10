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
                SurvivorProjectileState projectile = projectileEnumerator.Current.Value;
                projectile.PositionX += projectile.VelocityX;
                projectile.PositionY += projectile.VelocityY;
                projectile.RemainingTicks--;
                if (projectile.RemainingTicks <= 0)
                {
                    self.Runtime.ProjectileRemovalStateIds.Add(projectile.StateId);
                    continue;
                }

                bool hit = false;
                using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
                while (monsterEnumerator.MoveNext())
                {
                    SurvivorMonsterState monster = monsterEnumerator.Current.Value;
                    if (!monster.Alive)
                    {
                        continue;
                    }

                    if (SurvivorMath.Abs(projectile.PositionX - monster.PositionX) >
                        SurvivorDefaults.ProjectileCollisionRadius + SurvivorDefaults.MonsterCollisionRadius)
                    {
                        continue;
                    }

                    if (SurvivorMath.Abs(projectile.PositionY - monster.PositionY) >
                        SurvivorDefaults.ProjectileCollisionRadius + SurvivorDefaults.MonsterCollisionRadius)
                    {
                        continue;
                    }

                    monster.Hp -= projectile.Damage;
                    hit = true;
                    break;
                }

                if (hit)
                {
                    self.Runtime.ProjectileRemovalStateIds.Add(projectile.StateId);
                }
            }

            for (int index = 0; index < self.Runtime.ProjectileRemovalStateIds.Count; index++)
            {
                self.Data.Projectiles.Remove(self.Runtime.ProjectileRemovalStateIds[index]);
                self.Data.ProjectileSetRevision++;
            }
        }
    }
}
