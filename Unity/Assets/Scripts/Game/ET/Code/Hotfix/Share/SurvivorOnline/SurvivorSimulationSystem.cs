namespace ET
{
    public static class SurvivorSimulationSystem
    {
        /// <summary>
        /// 权威 tick。ObserveStateReactions 显式写在这里，顺序即代码顺序：
        /// 每一段施加完伤害/经验后立即结算，结算结果在同一 tick 内对后续段可见，
        /// 并且在房间广播快照之前全部落地。
        /// </summary>
        public static void TickAuthority(this SurvivorWorldComponent self)
        {
            if (self.Data.Phase != SurvivorRoomPhase.Running)
            {
                return;
            }

            self.Data.ServerTick++;
            self.TickPlayerMovement();
            if (self.Data.ServerTick % SurvivorDefaults.MonsterSpawnIntervalTicks == 0 && self.Data.Monsters.Count < SurvivorDefaults.MaxMonsters)
            {
                self.SpawnMonster();
            }

            self.TickMonsterMovementAndContact();
            self.ObserveStateReactions();
            if (self.Data.Phase != SurvivorRoomPhase.Running)
            {
                return;
            }

            self.TickWeapons();
            self.TickProjectiles();
            self.TickPickups();
            self.ObserveStateReactions();
        }

        /// <summary>
        /// 显式驱动状态反应观察。Children 是按 Entity Id 排序的 SortedDictionary，
        /// 因此观察者之间的顺序稳定可复现；先快照 Id 再逐个观察，避免 Bind 内部销毁观察者时破坏遍历。
        /// 驱动方只单向调用观察者 System，结算逻辑留在 SurvivorStateReactionSystem，避免 ET0013 环形依赖。
        /// </summary>
        private static void ObserveStateReactions(this SurvivorWorldComponent self)
        {
            self.Runtime.ObserverIds.Clear();
            self.Runtime.ObserverEnumerator = self.Children.Values.GetEnumerator();
            while (self.Runtime.ObserverEnumerator.MoveNext())
            {
                self.Runtime.ObserverIds.Add(self.Runtime.ObserverEnumerator.Current.Id);
            }

            self.Runtime.ObserverEnumerator.Dispose();
            self.Runtime.ObserverEnumerator = null;
            self.Runtime.ObserverIndex = 0;
            while (self.Runtime.ObserverIndex < self.Runtime.ObserverIds.Count)
            {
                self.ObserveStateReaction(self.Runtime.ObserverIds[self.Runtime.ObserverIndex]);
                self.Runtime.ObserverIndex++;
            }
        }

        private static void ObserveStateReaction(this SurvivorWorldComponent self, long observerId)
        {
            if (!self.Children.TryGetValue(observerId, out Entity child))
            {
                return;
            }

            if (child is SurvivorPlayerStateReactiveObserver playerObserver)
            {
                playerObserver.ObserveChanges();
                return;
            }

            SurvivorMonsterStateReactiveObserver monsterObserver = (SurvivorMonsterStateReactiveObserver)child;
            monsterObserver.ObserveChanges();
            if (!monsterObserver.State.Alive)
            {
                monsterObserver.Dispose();
            }
        }
    }
}
