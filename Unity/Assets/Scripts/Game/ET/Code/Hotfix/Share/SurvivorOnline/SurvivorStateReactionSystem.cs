namespace ET
{
    [ETReactiveSystem]
    [EntitySystemOf(typeof(SurvivorPlayerStateReactiveObserver))]
    public static partial class SurvivorPlayerStateReactiveObserverSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerStateReactiveObserver self, SurvivorPlayerState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorPlayerStateReactiveObserver self)
        {
            SurvivorPlayerState state = self.State;
            self.ResetReactive();
            self.State = null;
            state.LogicObserver = default;
        }

        [ETReactiveBind(nameof(SurvivorPlayerStateReactiveObserver.Experience))]
        private static void OnExperienceChanged(this SurvivorPlayerStateReactiveObserver self, int oldExperience, int newExperience)
        {
            self.GetParent<SurvivorWorldComponent>().ResolvePlayerExperienceChanged(self.State);
        }

        [ETReactiveBind(nameof(SurvivorPlayerStateReactiveObserver.Hp))]
        private static void OnHpChanged(this SurvivorPlayerStateReactiveObserver self, int oldHp, int newHp)
        {
            self.GetParent<SurvivorWorldComponent>().ResolvePlayerHpChanged(self.State, newHp);
        }
    }

    [ETReactiveSystem]
    [EntitySystemOf(typeof(SurvivorMonsterStateReactiveObserver))]
    public static partial class SurvivorMonsterStateReactiveObserverSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorMonsterStateReactiveObserver self, SurvivorMonsterState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Destroy(this SurvivorMonsterStateReactiveObserver self)
        {
            SurvivorMonsterState state = self.State;
            self.ResetReactive();
            self.State = null;
            state.LogicObserver = default;
        }

        [ETReactiveBind(nameof(SurvivorMonsterStateReactiveObserver.Hp))]
        private static void OnHpChanged(this SurvivorMonsterStateReactiveObserver self, int oldHp, int newHp)
        {
            self.GetParent<SurvivorWorldComponent>().ResolveMonsterHpChanged(self.State, newHp);
        }
    }

    public static class SurvivorStateReactionSystem
    {
        public static void AttachPlayerReaction(this SurvivorWorldComponent self, SurvivorPlayerState state)
        {
            state.LogicObserver = self.AddChild<SurvivorPlayerStateReactiveObserver, SurvivorPlayerState>(state);
        }

        public static void AttachMonsterReaction(this SurvivorWorldComponent self, SurvivorMonsterState state)
        {
            state.LogicObserver = self.AddChild<SurvivorMonsterStateReactiveObserver, SurvivorMonsterState>(state);
        }

        public static void DetachStateReactions(this SurvivorWorldComponent self)
        {
            using var playerEnumerator = self.Data.Players.GetEnumerator();
            while (playerEnumerator.MoveNext())
            {
                SurvivorPlayerStateReactiveObserver observer = playerEnumerator.Current.Value.LogicObserver;
                observer.Dispose();
            }

            using var monsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (monsterEnumerator.MoveNext())
            {
                SurvivorMonsterStateReactiveObserver observer = monsterEnumerator.Current.Value.LogicObserver;
                observer.Dispose();
            }
        }

        public static void ResolvePlayerHpChanged(this SurvivorWorldComponent self, SurvivorPlayerState state, int newHp)
        {
            if (newHp > state.MaxHp)
            {
                state.Hp = state.MaxHp;
                return;
            }

            if (newHp > 0)
            {
                return;
            }

            state.Hp = 0;
            state.Alive = false;
            state.MoveX = 0;
            state.MoveY = 0;
            self.CheckGameEnded();
        }

        public static void ResolvePlayerExperienceChanged(this SurvivorWorldComponent self, SurvivorPlayerState state)
        {
            while (state.Experience >= state.Level * SurvivorDefaults.LevelExperienceStep)
            {
                state.Experience -= state.Level * SurvivorDefaults.LevelExperienceStep;
                state.Level++;
                int oldMaxHp = state.MaxHp;
                state.MaxHp += SurvivorDefaults.LevelMaxHpIncrease;
                state.Hp = (int)((long)state.Hp * state.MaxHp / oldMaxHp);
                state.UnspentSkillPoints++;
            }

            if (state.UnspentSkillPoints > 0 && state.SkillChoice1 == SurvivorSkillType.None)
            {
                self.RefreshSkillChoices(state);
            }
        }

        public static void ResolveMonsterHpChanged(this SurvivorWorldComponent self, SurvivorMonsterState state, int newHp)
        {
            if (newHp > 0)
            {
                return;
            }

            state.Hp = 0;
            state.Alive = false;
            long stateId = self.AllocateStateId();
            self.Data.Pickups.Add(
                stateId,
                SurvivorWorldFactory.CreatePickup(
                    stateId,
                    state.PositionX,
                    state.PositionY,
                    SurvivorDefaults.MonsterKillExperience));
            self.Data.PickupSetRevision++;
            self.Data.Monsters.Remove(state.StateId);
            self.Data.MonsterSetRevision++;
        }

        public static void CheckGameEnded(this SurvivorWorldComponent self)
        {
            int alivePlayerCount = 0;
            using var playerEnumerator = self.Data.Players.GetEnumerator();
            while (playerEnumerator.MoveNext())
            {
                if (playerEnumerator.Current.Value.Alive)
                {
                    alivePlayerCount++;
                }
            }

            if (self.Data.Players.Count > 0 && alivePlayerCount == 0)
            {
                self.Data.Phase = SurvivorRoomPhase.Ended;
            }
        }
    }
}
