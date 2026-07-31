using ReactiveBinding;
using UnityEngine;

#pragma warning disable ET0004 // Stateful pooled ReactiveBinding observer intentionally lives in HotfixView.

namespace ET.Client
{
    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorPlayerUGFEntityReactiveObserver: IReactiveObserver, IPool
    {
        private EntityRef<SurvivorPlayerUGFEntity> entity;
        private SurvivorPlayerState state;

        public bool IsFromPool { get; set; }

        public static IReactiveObserver Create(SurvivorPlayerUGFEntity entity)
        {
            SurvivorPlayerUGFEntityReactiveObserver observer =
                    ObjectPool.Instance.Fetch<SurvivorPlayerUGFEntityReactiveObserver>();
            observer.ResetChanges();
            observer.entity = entity;
            observer.state = entity.GetParent<SurvivorPlayerEntry>()
                    .GetParent<SurvivorClientComponent>()
                    .Runtime
                    .PlayerStates[entity.GetParent<SurvivorPlayerEntry>().Id];
            observer.InitializeView();
            return observer;
        }

        public static void Recycle(IReactiveObserver observer)
        {
            if (observer is not SurvivorPlayerUGFEntityReactiveObserver playerObserver)
            {
                return;
            }

            playerObserver.ResetChanges();
            playerObserver.entity = default;
            playerObserver.state = null;
            ObjectPool.Instance.Recycle(playerObserver);
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
            ((SurvivorPlayerUGFEntity)this.entity).CachedTransform.position =
                    new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }

        private void InitializeView()
        {
            ((SurvivorPlayerUGFEntity)this.entity).View.SpriteRenderer.color =
                    new Color(0.2f, 0.55f, 1f, 1f);
            ((SurvivorPlayerUGFEntity)this.entity).View.SpriteRenderer.sortingOrder = 20;
            ((SurvivorPlayerUGFEntity)this.entity).CachedTransform.localScale =
                    new Vector3(1f, 1f, 1f);
        }
    }
}

#pragma warning restore ET0004
