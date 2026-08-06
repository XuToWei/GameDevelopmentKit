using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(SurvivorPlayerEntry))]
    public sealed partial class SurvivorPlayerUGFEntity:
            UGFEntity<MonoSurvivorSpriteEntity>,
            IAwake,
            IUGFEntityOnShow,
            IUGFEntityOnUpdate,
            IUGFEntityOnHide,
            IETReactive
    {
        /// <summary>Entry 在本组件生命周期内稳定，State 由 Entry 实时提供，快照重建实例后不会拿到旧引用。</summary>
        public SurvivorPlayerEntry Entry { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        public bool IsLocalPlayer { get; set; }

        public GameObject SwordWaveVisual { get; set; }

        public SpriteRenderer SwordWaveRenderer { get; set; }

        public float SwordWaveVisualRemainingSeconds { get; set; }

        [ETReactiveSource]
        public int PositionX => this.Entry.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.Entry.State.PositionY;

        [ETReactiveSource]
        public long SwordWaveRevision => this.Entry.State.SwordWaveRevision;
    }
}
