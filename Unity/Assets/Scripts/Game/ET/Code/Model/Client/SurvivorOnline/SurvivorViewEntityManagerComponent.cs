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

        /// <summary>父节点在生命周期内稳定，按经验文档第 5 章在 Awake 缓存。</summary>
        public SurvivorClientComponent Client { get; set; }

        [ETReactiveSource]
        public long WorldGeneration { get; set; }

        [ETReactiveSource]
        public long PlayerSetRevision => this.Client.HasBaseline ? this.Client.WorldComponent.Data.PlayerSetRevision : 0;

        [ETReactiveSource]
        public long MonsterSetRevision => this.Client.HasBaseline ? this.Client.WorldComponent.Data.MonsterSetRevision : 0;

        [ETReactiveSource]
        public long ProjectileSetRevision => this.Client.HasBaseline ? this.Client.WorldComponent.Data.ProjectileSetRevision : 0;

        [ETReactiveSource]
        public long PickupSetRevision => this.Client.HasBaseline ? this.Client.WorldComponent.Data.PickupSetRevision : 0;
    }

    [EnableClass]
    public sealed class SurvivorViewEntityManagerRuntime
    {
        public IEnumerator<KeyValuePair<long, SurvivorPlayerState>> PlayerEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorMonsterState>> MonsterEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorProjectileState>> ProjectileEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorPickupState>> PickupEnumerator { get; set; }

        public IEnumerator<Entity> EntryEnumerator { get; set; }

        public HashSet<long> SeenStateIds { get; } = new();

        public List<long> RemovalStateIds { get; } = new();

        public long AppliedWorldGeneration { get; set; }

        public int Index { get; set; }
    }
}
