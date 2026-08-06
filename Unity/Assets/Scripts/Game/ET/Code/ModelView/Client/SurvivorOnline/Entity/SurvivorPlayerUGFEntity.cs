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
        public SurvivorPlayerState State { get; set; }

        public SurvivorPresentationPosition PresentationPosition { get; set; }

        public bool IsLocalPlayer { get; set; }

        public GameObject SwordWaveVisual { get; set; }

        public SpriteRenderer SwordWaveRenderer { get; set; }

        public float SwordWaveVisualRemainingSeconds { get; set; }

        [ETReactiveSource]
        public int PositionX => this.State.PositionX;

        [ETReactiveSource]
        public int PositionY => this.State.PositionY;
    
        [ETReactiveSource]
        public long SwordWaveRevision => this.State.SwordWaveRevision;
    }
}
