namespace ET
{
    public static class SurvivorSimulationSystem
    {
        public static void TickAuthority(this SurvivorWorldComponent self)
        {
            if (self.Data.Phase != SurvivorRoomPhase.Running)
            {
                return;
            }

            self.Data.ServerTick++;
            self.TickPlayerMovement();
            if (self.Data.ServerTick % SurvivorDefaults.MonsterSpawnIntervalTicks == 0 &&
                self.Data.Monsters.Count < SurvivorDefaults.MaxMonsters)
            {
                self.SpawnMonster();
            }

            self.TickMonsterMovementAndContact();
            if (self.Data.Phase != SurvivorRoomPhase.Running)
            {
                return;
            }

            self.TickWeapons();
            self.TickProjectiles();
            self.TickPickups();
        }
    }
}
