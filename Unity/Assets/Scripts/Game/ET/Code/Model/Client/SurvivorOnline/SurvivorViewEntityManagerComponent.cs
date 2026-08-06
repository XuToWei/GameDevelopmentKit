using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf(typeof(SurvivorClientComponent))]
    public sealed partial class SurvivorViewEntityManagerComponent:
            Entity,
            IAwake,
            IUpdate,
            IDestroy,
            IETReactive
    {
        public SurvivorViewEntityManagerRuntime Runtime { get; set; }

        [ETReactiveSource]
        public long WorldGeneration { get; set; }

        public SurvivorClientComponent Client => this.GetParent<SurvivorClientComponent>();

        public SurvivorWorldComponent WorldComponent => this.Client.World;

        [ETReactiveSource]
        public long PlayerSetRevision => this.Client.HasBaseline ? this.WorldComponent.Data.PlayerSetRevision : 0;

        [ETReactiveSource]
        public long MonsterSetRevision => this.Client.HasBaseline ? this.WorldComponent.Data.MonsterSetRevision : 0;

        [ETReactiveSource]
        public long ProjectileSetRevision => this.Client.HasBaseline ? this.WorldComponent.Data.ProjectileSetRevision : 0;

        [ETReactiveSource]
        public long PickupSetRevision => this.Client.HasBaseline ? this.WorldComponent.Data.PickupSetRevision : 0;
    }

    [EnableClass]
    public sealed class SurvivorViewEntityManagerRuntime
    {
        public Dictionary<long, SurvivorPlayerState> PlayerStates { get; } = new();

        public Dictionary<long, SurvivorMonsterState> MonsterStates { get; } = new();

        public Dictionary<long, SurvivorProjectileState> ProjectileStates { get; } = new();

        public Dictionary<long, SurvivorPickupState> PickupStates { get; } = new();

        public IEnumerator<KeyValuePair<long, SurvivorPlayerState>> PlayerEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorMonsterState>> MonsterEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorProjectileState>> ProjectileEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorPickupState>> PickupEnumerator { get; set; }

        public IEnumerator<Entity> EntryEnumerator { get; set; }

        public HashSet<long> SeenStateIds { get; } = new();

        public List<long> RemovalStateIds { get; } = new();

        public long AppliedWorldGeneration { get; set; }

        public long StateId { get; set; }

        public int Index { get; set; }
    }
}
