using ReactiveBinding;

namespace ET
{
    public interface ISurvivorPlayerReactionSink
    {
        void OnHpChanged(
            SurvivorWorldComponent world,
            SurvivorPlayerState state,
            int oldHp,
            int newHp);

        void OnExperienceChanged(
            SurvivorWorldComponent world,
            SurvivorPlayerState state,
            int oldExperience,
            int newExperience);
    }

    public interface ISurvivorMonsterReactionSink
    {
        void OnHpChanged(
            SurvivorWorldComponent world,
            SurvivorMonsterState state,
            int oldHp,
            int newHp);
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorPlayerStateReactiveObserver: IReactiveObserver
    {
        private EntityRef<SurvivorWorldComponent> world;
        private SurvivorPlayerState state;
        private ISurvivorPlayerReactionSink sink;

        public SurvivorPlayerStateReactiveObserver(
            SurvivorWorldComponent world,
            SurvivorPlayerState state,
            ISurvivorPlayerReactionSink sink)
        {
            this.world = world;
            this.state = state;
            this.sink = sink;
        }

        [ReactiveSource]
        private int Hp
        {
            get
            {
                return this.state.Hp;
            }
        }

        [ReactiveSource]
        private int Experience
        {
            get
            {
                return this.state.Experience;
            }
        }

        [ReactiveBind(nameof(Hp))]
        private void OnHpChanged(int oldHp, int newHp)
        {
            this.sink.OnHpChanged(this.world, this.state, oldHp, newHp);
        }

        [ReactiveBind(nameof(Experience))]
        private void OnExperienceChanged(int oldExperience, int newExperience)
        {
            this.sink.OnExperienceChanged(this.world, this.state, oldExperience, newExperience);
        }
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorMonsterStateReactiveObserver: IReactiveObserver
    {
        private EntityRef<SurvivorWorldComponent> world;
        private SurvivorMonsterState state;
        private ISurvivorMonsterReactionSink sink;

        public SurvivorMonsterStateReactiveObserver(
            SurvivorWorldComponent world,
            SurvivorMonsterState state,
            ISurvivorMonsterReactionSink sink)
        {
            this.world = world;
            this.state = state;
            this.sink = sink;
        }

        [ReactiveSource]
        private int Hp
        {
            get
            {
                return this.state.Hp;
            }
        }

        [ReactiveBind(nameof(Hp))]
        private void OnHpChanged(int oldHp, int newHp)
        {
            this.sink.OnHpChanged(this.world, this.state, oldHp, newHp);
        }
    }
}
