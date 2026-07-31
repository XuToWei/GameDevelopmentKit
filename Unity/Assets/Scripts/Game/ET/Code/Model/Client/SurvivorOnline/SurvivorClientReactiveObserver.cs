using ReactiveBinding;

namespace ET.Client
{
    public interface ISurvivorClientReactionSink
    {
        void OnMembershipChanged(SurvivorClientComponent client);
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorClientReactiveObserver: IReactiveObserver
    {
        private EntityRef<SurvivorClientComponent> client;
        private ISurvivorClientReactionSink sink;

        public SurvivorClientReactiveObserver(
            SurvivorClientComponent client,
            ISurvivorClientReactionSink sink)
        {
            this.client = client;
            this.sink = sink;
        }

        private SurvivorClientComponent Client
        {
            get
            {
                return this.client;
            }
        }

        private SurvivorWorldData WorldData
        {
            get
            {
                return this.Client.Room?.GetComponent<SurvivorWorldComponent>()?.Data;
            }
        }

        [ReactiveSource]
        private long PlayerSetRevision
        {
            get
            {
                return this.WorldData?.PlayerSetRevision ?? 0;
            }
        }

        [ReactiveSource]
        private long MonsterSetRevision
        {
            get
            {
                return this.WorldData?.MonsterSetRevision ?? 0;
            }
        }

        [ReactiveSource]
        private long ProjectileSetRevision
        {
            get
            {
                return this.WorldData?.ProjectileSetRevision ?? 0;
            }
        }

        [ReactiveSource]
        private long PickupSetRevision
        {
            get
            {
                return this.WorldData?.PickupSetRevision ?? 0;
            }
        }

        [ReactiveBind(
            nameof(PlayerSetRevision),
            nameof(MonsterSetRevision),
            nameof(ProjectileSetRevision),
            nameof(PickupSetRevision))]
        private void OnMembershipChanged()
        {
            this.sink.OnMembershipChanged(this.Client);
        }
    }
}
