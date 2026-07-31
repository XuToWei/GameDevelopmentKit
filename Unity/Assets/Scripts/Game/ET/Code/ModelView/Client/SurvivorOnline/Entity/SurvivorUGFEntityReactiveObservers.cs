using ReactiveBinding;

namespace ET.Client
{
    public interface ISurvivorUGFEntityReactionSink
    {
        void OnMonsterPositionChanged(SurvivorMonsterUGFEntity entity, int positionX, int positionY);

        void OnProjectilePositionChanged(SurvivorProjectileUGFEntity entity, int positionX, int positionY);

        void OnPickupPositionChanged(SurvivorPickupUGFEntity entity, int positionX, int positionY);
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorMonsterUGFEntityReactiveObserver: IReactiveObserver
    {
        private EntityRef<SurvivorMonsterUGFEntity> entity;
        private SurvivorMonsterState state;
        private ISurvivorUGFEntityReactionSink sink;

        public SurvivorMonsterUGFEntityReactiveObserver(
            SurvivorMonsterUGFEntity entity,
            SurvivorMonsterState state,
            ISurvivorUGFEntityReactionSink sink)
        {
            this.entity = entity;
            this.state = state;
            this.sink = sink;
        }

        [ReactiveSource]
        private int PositionX
        {
            get
            {
                return this.state.PositionX;
            }
        }

        [ReactiveSource]
        private int PositionY
        {
            get
            {
                return this.state.PositionY;
            }
        }

        [ReactiveBind(nameof(PositionX), nameof(PositionY))]
        private void OnPositionChanged(int positionX, int positionY)
        {
            this.sink.OnMonsterPositionChanged(this.entity, positionX, positionY);
        }
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorProjectileUGFEntityReactiveObserver: IReactiveObserver
    {
        private EntityRef<SurvivorProjectileUGFEntity> entity;
        private SurvivorProjectileState state;
        private ISurvivorUGFEntityReactionSink sink;

        public SurvivorProjectileUGFEntityReactiveObserver(
            SurvivorProjectileUGFEntity entity,
            SurvivorProjectileState state,
            ISurvivorUGFEntityReactionSink sink)
        {
            this.entity = entity;
            this.state = state;
            this.sink = sink;
        }

        [ReactiveSource]
        private int PositionX
        {
            get
            {
                return this.state.PositionX;
            }
        }

        [ReactiveSource]
        private int PositionY
        {
            get
            {
                return this.state.PositionY;
            }
        }

        [ReactiveBind(nameof(PositionX), nameof(PositionY))]
        private void OnPositionChanged(int positionX, int positionY)
        {
            this.sink.OnProjectilePositionChanged(this.entity, positionX, positionY);
        }
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorPickupUGFEntityReactiveObserver: IReactiveObserver
    {
        private EntityRef<SurvivorPickupUGFEntity> entity;
        private SurvivorPickupState state;
        private ISurvivorUGFEntityReactionSink sink;

        public SurvivorPickupUGFEntityReactiveObserver(
            SurvivorPickupUGFEntity entity,
            SurvivorPickupState state,
            ISurvivorUGFEntityReactionSink sink)
        {
            this.entity = entity;
            this.state = state;
            this.sink = sink;
        }

        [ReactiveSource]
        private int PositionX
        {
            get
            {
                return this.state.PositionX;
            }
        }

        [ReactiveSource]
        private int PositionY
        {
            get
            {
                return this.state.PositionY;
            }
        }

        [ReactiveBind(nameof(PositionX), nameof(PositionY))]
        private void OnPositionChanged(int positionX, int positionY)
        {
            this.sink.OnPickupPositionChanged(this.entity, positionX, positionY);
        }
    }
}
