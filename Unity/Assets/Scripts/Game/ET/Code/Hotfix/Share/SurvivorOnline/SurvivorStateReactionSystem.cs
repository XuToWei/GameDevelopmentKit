namespace ET
{
    [EntitySystemOf(typeof(SurvivorPlayerStateReactiveObserver))]
    [ETReactiveSystem]
    public static partial class SurvivorPlayerStateReactiveObserverSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerStateReactiveObserver self, SurvivorPlayerState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Update(this SurvivorPlayerStateReactiveObserver self)
        {
            self.ObserveChanges();
        }

        [EntitySystem]
        private static void Destroy(this SurvivorPlayerStateReactiveObserver self)
        {
            SurvivorPlayerState state = self.State;
            self.ClearReactive();
            self.State = null;
            if (state != null)
            {
                state.LogicObserver = default;
            }
        }

        [ETReactiveBind(nameof(SurvivorPlayerStateReactiveObserver.Hp))]
        private static void OnHpChanged(this SurvivorPlayerStateReactiveObserver self, int oldHp, int newHp)
        {
            self.GetParent<SurvivorWorldComponent>().ResolvePlayerHpChanged(self.State, newHp);
        }

        [ETReactiveBind(nameof(SurvivorPlayerStateReactiveObserver.Experience))]
        private static void OnExperienceChanged(this SurvivorPlayerStateReactiveObserver self, int oldExperience, int newExperience)
        {
            self.GetParent<SurvivorWorldComponent>().ResolvePlayerExperienceChanged(self.State);
        }
    }

    [EntitySystemOf(typeof(SurvivorMonsterStateReactiveObserver))]
    [ETReactiveSystem]
    public static partial class SurvivorMonsterStateReactiveObserverSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorMonsterStateReactiveObserver self, SurvivorMonsterState state)
        {
            self.State = state;
        }

        [EntitySystem]
        private static void Update(this SurvivorMonsterStateReactiveObserver self)
        {
            self.ObserveChanges();
            if (!self.State.Alive)
            {
                self.Dispose();
            }
        }

        [EntitySystem]
        private static void Destroy(this SurvivorMonsterStateReactiveObserver self)
        {
            SurvivorMonsterState state = self.State;
            self.ClearReactive();
            self.State = null;
            if (state != null)
            {
                state.LogicObserver = default;
            }
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
            SurvivorPlayerStateReactiveObserver observer = state.LogicObserver;
            if (observer != null)
            {
                observer.Dispose();
            }

            state.LogicObserver = self.AddChild<SurvivorPlayerStateReactiveObserver, SurvivorPlayerState>(state);
        }

        public static void AttachMonsterReaction(this SurvivorWorldComponent self, SurvivorMonsterState state)
        {
            SurvivorMonsterStateReactiveObserver observer = state.LogicObserver;
            if (observer != null)
            {
                observer.Dispose();
            }

            state.LogicObserver = self.AddChild<SurvivorMonsterStateReactiveObserver, SurvivorMonsterState>(state);
        }

        public static void DetachStateReactions(this SurvivorWorldComponent self)
        {
            self.Runtime.PlayerEnumerator = self.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                SurvivorPlayerStateReactiveObserver observer = self.Runtime.PlayerEnumerator.Current.Value.LogicObserver;
                observer?.Dispose();
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;

            self.Runtime.MonsterEnumerator = self.Data.Monsters.GetEnumerator();
            while (self.Runtime.MonsterEnumerator.MoveNext())
            {
                SurvivorMonsterStateReactiveObserver observer = self.Runtime.MonsterEnumerator.Current.Value.LogicObserver;
                observer?.Dispose();
            }

            self.Runtime.MonsterEnumerator.Dispose();
            self.Runtime.MonsterEnumerator = null;
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
                state.MaxHp += 10;
                state.Hp = (int)((long)state.Hp * state.MaxHp / (state.MaxHp - 10));
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
            self.Runtime.StateId = self.AllocateStateId();
            self.Data.Pickups.Add(self.Runtime.StateId, SurvivorWorldFactory.CreatePickup(self.Runtime.StateId, state.PositionX, state.PositionY, SurvivorDefaults.MonsterKillExperience));
            self.Data.PickupSetRevision++;
            self.Data.Monsters.Remove(state.StateId);
            self.Data.MonsterSetRevision++;
        }

        public static void CheckGameEnded(this SurvivorWorldComponent self)
        {
            self.Runtime.AlivePlayerCount = 0;
            self.Runtime.PlayerEnumerator = self.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                if (self.Runtime.PlayerEnumerator.Current.Value.Alive)
                {
                    self.Runtime.AlivePlayerCount++;
                }
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;
            if (self.Data.Players.Count > 0 && self.Runtime.AlivePlayerCount == 0)
            {
                self.Data.Phase = SurvivorRoomPhase.Ended;
            }
        }
    }
}
