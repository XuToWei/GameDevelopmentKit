namespace ET
{
    public static class SurvivorWeaponSystem
    {
        public static void TickAutoFire(this SurvivorWorldComponent self)
        {
            self.Runtime.PlayerEnumerator = self.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                self.Runtime.Player = self.Runtime.PlayerEnumerator.Current.Value;
                if (!self.Runtime.Player.Alive)
                {
                    continue;
                }

                if (self.Runtime.Player.AutoFireCooldown > 0)
                {
                    self.Runtime.Player.AutoFireCooldown--;
                }

                if (self.Runtime.Player.AutoFireCooldown > 0)
                {
                    continue;
                }

                self.Runtime.TargetMonster = null;
                self.Runtime.Distance = int.MaxValue;
                self.Runtime.MonsterEnumerator = self.Data.Monsters.GetEnumerator();
                while (self.Runtime.MonsterEnumerator.MoveNext())
                {
                    if (!self.Runtime.MonsterEnumerator.Current.Value.Alive)
                    {
                        continue;
                    }

                    self.Runtime.DeltaX =
                            self.Runtime.MonsterEnumerator.Current.Value.PositionX - self.Runtime.Player.PositionX;
                    self.Runtime.DeltaY =
                            self.Runtime.MonsterEnumerator.Current.Value.PositionY - self.Runtime.Player.PositionY;
                    if (SurvivorMath.Abs(self.Runtime.DeltaX) + SurvivorMath.Abs(self.Runtime.DeltaY) >=
                        self.Runtime.Distance)
                    {
                        continue;
                    }

                    self.Runtime.Distance =
                            SurvivorMath.Abs(self.Runtime.DeltaX) + SurvivorMath.Abs(self.Runtime.DeltaY);
                    self.Runtime.TargetMonster = self.Runtime.MonsterEnumerator.Current.Value;
                }

                self.Runtime.MonsterEnumerator.Dispose();
                self.Runtime.MonsterEnumerator = null;
                if (self.Runtime.TargetMonster == null)
                {
                    continue;
                }

                self.Runtime.DeltaX = self.Runtime.TargetMonster.PositionX - self.Runtime.Player.PositionX;
                self.Runtime.DeltaY = self.Runtime.TargetMonster.PositionY - self.Runtime.Player.PositionY;
                self.Runtime.VelocityX =
                        SurvivorMath.Sign(self.Runtime.DeltaX) * SurvivorDefaults.ProjectileMovePerTick;
                self.Runtime.VelocityY =
                        SurvivorMath.Sign(self.Runtime.DeltaY) * SurvivorDefaults.ProjectileMovePerTick;
                if (self.Runtime.VelocityX == 0 && self.Runtime.VelocityY == 0)
                {
                    self.Runtime.VelocityX = SurvivorDefaults.ProjectileMovePerTick;
                }

                self.Runtime.StateId = self.AllocateStateId();
                self.Data.Projectiles.Add(
                    self.Runtime.StateId,
                    SurvivorWorldFactory.CreateProjectile(
                        self.Runtime.StateId,
                        self.Runtime.Player.PlayerId,
                        self.Runtime.Player.PositionX,
                        self.Runtime.Player.PositionY,
                        self.Runtime.VelocityX,
                        self.Runtime.VelocityY));
                self.Data.ProjectileSetRevision++;
                self.Runtime.Player.AutoFireCooldown = SurvivorDefaults.AutoFireIntervalTicks;
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;
            self.Runtime.Player = null;
            self.Runtime.TargetMonster = null;
        }
    }
}
