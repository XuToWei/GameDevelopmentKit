using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public sealed partial class SurvivorClientComponent: Entity, IAwake, IDestroy, IETReactiveHost
    {
        private EntityRef<SurvivorRoom> room;
        private EntityRef<SurvivorWorldComponent> world;

        public SurvivorRoom Room
        {
            get
            {
                return this.room;
            }
            set
            {
                this.room = value;
            }
        }

        public SurvivorWorldComponent World
        {
            get
            {
                return this.world;
            }
            set
            {
                this.world = value;
            }
        }

        public SurvivorClientRuntime Runtime { get; set; }

        public long PlayerId { get; set; }

        public long LastSequence { get; set; }

        public long InputSequence { get; set; }

        public bool IsHost { get; set; }

        public bool HasBaseline { get; set; }
    }

    [EnableClass]
    public sealed class SurvivorClientRuntime
    {
        public C2G_SurvivorJoinRoom JoinRequest { get; set; }

        public G2C_SurvivorJoinRoom JoinResponse { get; set; }

        public C2G_SurvivorStartGame StartRequest { get; set; }

        public G2C_SurvivorStartGame StartResponse { get; set; }

        public C2G_SurvivorInput InputMessage { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorPlayerState>> PlayerEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorMonsterState>> MonsterEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorProjectileState>> ProjectileEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorPickupState>> PickupEnumerator { get; set; }

        public IEnumerator<Entity> EntryEnumerator { get; set; }

        public Dictionary<long, SurvivorPlayerState> PlayerStates { get; } = new();

        public Dictionary<long, SurvivorMonsterState> MonsterStates { get; } = new();

        public Dictionary<long, SurvivorProjectileState> ProjectileStates { get; } = new();

        public Dictionary<long, SurvivorPickupState> PickupStates { get; } = new();

        public HashSet<long> SeenStateIds { get; } = new();

        public List<long> RemovalStateIds { get; } = new();

        public long StateId { get; set; }

        public int Index { get; set; }
    }
}
