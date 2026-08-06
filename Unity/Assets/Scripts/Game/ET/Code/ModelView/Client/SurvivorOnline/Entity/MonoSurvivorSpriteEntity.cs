using CodeBind;
using UnityEngine;

namespace ET.Client
{
    [MonoBehaviourBinding]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed partial class MonoSurvivorSpriteEntity: AETMonoUGFEntity
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public SpriteRenderer SpriteRenderer => this.spriteRenderer;
    }
}
