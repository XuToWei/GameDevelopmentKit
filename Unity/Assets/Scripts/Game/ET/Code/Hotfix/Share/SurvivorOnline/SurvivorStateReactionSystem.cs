namespace ET
{
    [EnableClass]
    public sealed class SurvivorPlayerReactionSink: ISurvivorPlayerReactionSink
    {
        public void OnHpChanged(
            SurvivorWorldComponent world,
            SurvivorPlayerState state,
            int oldHp,
            int newHp)
        {
            world.ResolvePlayerHpChanged(state, newHp);
        }

        public void OnExperienceChanged(
            SurvivorWorldComponent world,
            SurvivorPlayerState state,
            int oldExperience,
            int newExperience)
        {
            world.ResolvePlayerExperienceChanged(state);
        }
    }

    [EnableClass]
    public sealed class SurvivorMonsterReactionSink: ISurvivorMonsterReactionSink
    {
        public void OnHpChanged(
            SurvivorWorldComponent world,
            SurvivorMonsterState state,
            int oldHp,
            int newHp)
        {
            world.ResolveMonsterHpChanged(state, newHp);
        }
    }

    public static class SurvivorStateReactionSystem
    {
        public static void AttachPlayerReaction(
            this SurvivorWorldComponent self,
            SurvivorPlayerState state)
        {
            state.LogicObserver = new SurvivorPlayerStateReactiveObserver(
                self,
                state,
                self.Runtime.PlayerReactionSink);
            state.LogicObserver.ResetChanges();
            state.LogicObserver.ObserveChanges();
        }

        public static void AttachMonsterReaction(
            this SurvivorWorldComponent self,
            SurvivorMonsterState state)
        {
            state.LogicObserver = new SurvivorMonsterStateReactiveObserver(
                self,
                state,
                self.Runtime.MonsterReactionSink);
            state.LogicObserver.ResetChanges();
            state.LogicObserver.ObserveChanges();
        }

        public static void ResolvePlayerHpChanged(
            this SurvivorWorldComponent self,
            SurvivorPlayerState state,
            int newHp)
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

        public static void ResolvePlayerExperienceChanged(
            this SurvivorWorldComponent self,
            SurvivorPlayerState state)
        {
            while (state.Experience >= state.Level * SurvivorDefaults.LevelExperienceStep)
            {
                state.Experience -= state.Level * SurvivorDefaults.LevelExperienceStep;
                state.Level++;
                state.MaxHp += 10;
                state.Hp = state.MaxHp;
            }
        }

        public static void ResolveMonsterHpChanged(
            this SurvivorWorldComponent self,
            SurvivorMonsterState state,
            int newHp)
        {
            if (newHp > 0)
            {
                return;
            }

            state.Hp = 0;
            state.Alive = false;
            self.Runtime.StateId = self.AllocateStateId();
            self.Data.Pickups.Add(
                self.Runtime.StateId,
                SurvivorWorldFactory.CreatePickup(
                    self.Runtime.StateId,
                    state.PositionX,
                    state.PositionY,
                    SurvivorDefaults.MonsterKillExperience));
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
