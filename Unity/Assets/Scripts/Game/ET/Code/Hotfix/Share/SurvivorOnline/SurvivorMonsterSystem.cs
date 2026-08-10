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

            int spawnPositionX;
            int spawnPositionY;
            if ((self.NextRandom() & 1) == 0)
            {
                spawnPositionX = (self.NextRandom() & 1) == 0 ? -SurvivorDefaults.SpawnDistance : SurvivorDefaults.SpawnDistance;
                spawnPositionY = self.NextRandom() % (SurvivorDefaults.SpawnDistance * 2 + 1) - SurvivorDefaults.SpawnDistance;
            }
            else
            {
                spawnPositionX = self.NextRandom() % (SurvivorDefaults.SpawnDistance * 2 + 1) - SurvivorDefaults.SpawnDistance;
                spawnPositionY = (self.NextRandom() & 1) == 0 ? -SurvivorDefaults.SpawnDistance : SurvivorDefaults.SpawnDistance;
            }

            long stateId = self.AllocateStateId();
            SurvivorMonsterState monster = SurvivorWorldFactory.CreateMonster(stateId, spawnPositionX, spawnPositionY);
            self.Data.Monsters.Add(stateId, monster);
            self.Data.MonsterSetRevision++;
            self.AttachMonsterReaction(monster);
        }

        public static void TickMonsterMovementAndContact(this SurvivorWorldComponent self)
        {
            using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (monsterEnumerator.MoveNext())
            {
                SurvivorMonsterState monster = monsterEnumerator.Current.Value;
                if (!monster.Alive)
                {
                    continue;
                }

                SurvivorPlayerState targetPlayer = null;
                int nearestDistance = int.MaxValue;
                using var playerEnumerator = self.Data.Players.GetEnumerator();
                while (playerEnumerator.MoveNext())
                {
                    SurvivorPlayerState player = playerEnumerator.Current.Value;
                    if (!player.Alive)
                    {
                        continue;
                    }

                    int deltaX = player.PositionX - monster.PositionX;
                    int deltaY = player.PositionY - monster.PositionY;
                    int distance = SurvivorMath.Abs(deltaX) + SurvivorMath.Abs(deltaY);
                    if (distance >= nearestDistance)
                    {
                        continue;
                    }

                    nearestDistance = distance;
                    targetPlayer = player;
                }

                if (targetPlayer == null)
                {
                    continue;
                }

                monster.TargetPlayerId = targetPlayer.PlayerId;
                int targetDeltaX = targetPlayer.PositionX - monster.PositionX;
                int targetDeltaY = targetPlayer.PositionY - monster.PositionY;
                monster.PositionX += SurvivorMath.Sign(targetDeltaX) * SurvivorDefaults.MonsterMovePerTick;
                monster.PositionY += SurvivorMath.Sign(targetDeltaY) * SurvivorDefaults.MonsterMovePerTick;
                if (SurvivorMath.Abs(targetDeltaX) <= SurvivorDefaults.PlayerCollisionRadius + SurvivorDefaults.MonsterCollisionRadius &&
                    SurvivorMath.Abs(targetDeltaY) <= SurvivorDefaults.PlayerCollisionRadius + SurvivorDefaults.MonsterCollisionRadius)
                {
                    targetPlayer.Hp -= SurvivorDefaults.MonsterContactDamagePerTick;
                }
            }
        }
    }
}
