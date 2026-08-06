namespace ET
{
    public static class SurvivorMonsterSystem
    {
        public static void SpawnMonster(this SurvivorWorldComponent self)
        {
            if (self.Data.Players.Count == 0)
            {
                return;
            }

            self.Runtime.Distance = self.NextRandom();
            if ((self.Runtime.Distance & 1) == 0)
            {
                self.Runtime.SpawnPositionX =
                        (self.NextRandom() & 1) == 0 ? -SurvivorDefaults.SpawnDistance : SurvivorDefaults.SpawnDistance;
                self.Runtime.SpawnPositionY =
                        self.NextRandom() % (SurvivorDefaults.SpawnDistance * 2 + 1) - SurvivorDefaults.SpawnDistance;
            }
            else
            {
                self.Runtime.SpawnPositionX =
                        self.NextRandom() % (SurvivorDefaults.SpawnDistance * 2 + 1) - SurvivorDefaults.SpawnDistance;
                self.Runtime.SpawnPositionY =
                        (self.NextRandom() & 1) == 0 ? -SurvivorDefaults.SpawnDistance : SurvivorDefaults.SpawnDistance;
            }

            self.Runtime.StateId = self.AllocateStateId();
            self.Data.Monsters.Add(
                self.Runtime.StateId,
                SurvivorWorldFactory.CreateMonster(
                    self.Runtime.StateId,
                    self.Runtime.SpawnPositionX,
                    self.Runtime.SpawnPositionY));
            self.Data.MonsterSetRevision++;
            self.AttachMonsterReaction(self.Data.Monsters[self.Runtime.StateId]);
        }

        public static void TickMonsterMovementAndContact(this SurvivorWorldComponent self)
        {
            using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (monsterEnumerator.MoveNext())
            {
                self.Runtime.Monster = monsterEnumerator.Current.Value;
                if (!self.Runtime.Monster.Alive)
                {
                    continue;
                }

                self.Runtime.TargetPlayer = null;
                self.Runtime.Distance = int.MaxValue;
                using var playerEnumerator = self.Data.Players.GetEnumerator();
                while (playerEnumerator.MoveNext())
                {
                    if (!playerEnumerator.Current.Value.Alive)
                    {
                        continue;
                    }

                    self.Runtime.DeltaX =
                            playerEnumerator.Current.Value.PositionX - self.Runtime.Monster.PositionX;
                    self.Runtime.DeltaY =
                            playerEnumerator.Current.Value.PositionY - self.Runtime.Monster.PositionY;
                    if (SurvivorMath.Abs(self.Runtime.DeltaX) + SurvivorMath.Abs(self.Runtime.DeltaY) >=
                        self.Runtime.Distance)
                    {
                        continue;
                    }

                    self.Runtime.Distance =
                            SurvivorMath.Abs(self.Runtime.DeltaX) + SurvivorMath.Abs(self.Runtime.DeltaY);
                    self.Runtime.TargetPlayer = playerEnumerator.Current.Value;
                }

                if (self.Runtime.TargetPlayer == null)
                {
                    continue;
                }

                self.Runtime.Monster.TargetPlayerId = self.Runtime.TargetPlayer.PlayerId;
                self.Runtime.DeltaX = self.Runtime.TargetPlayer.PositionX - self.Runtime.Monster.PositionX;
                self.Runtime.DeltaY = self.Runtime.TargetPlayer.PositionY - self.Runtime.Monster.PositionY;
                self.Runtime.Monster.PositionX +=
                        SurvivorMath.Sign(self.Runtime.DeltaX) * SurvivorDefaults.MonsterMovePerTick;
                self.Runtime.Monster.PositionY +=
                        SurvivorMath.Sign(self.Runtime.DeltaY) * SurvivorDefaults.MonsterMovePerTick;
                if (SurvivorMath.Abs(self.Runtime.DeltaX) <=
                    SurvivorDefaults.PlayerCollisionRadius + SurvivorDefaults.MonsterCollisionRadius &&
                    SurvivorMath.Abs(self.Runtime.DeltaY) <=
                    SurvivorDefaults.PlayerCollisionRadius + SurvivorDefaults.MonsterCollisionRadius)
                {
                    self.Runtime.TargetPlayer.Hp -= SurvivorDefaults.MonsterContactDamagePerTick;
                }
            }

            self.Runtime.Monster = null;
            self.Runtime.TargetPlayer = null;
        }
    }
}
