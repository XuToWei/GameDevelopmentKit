using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorProjectileUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorProjectileUGFEntitySystem
    {
        private const float VisualScale = 3.125f;

        [EntitySystem]
        private static void Awake(this SurvivorProjectileUGFEntity self)
        {
            self.PresentationPosition = new SurvivorPresentationPosition();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorProjectileUGFEntity self)
        {
            self.Entry = self.GetParent<SurvivorProjectileEntry>();
            self.View.SpriteRenderer.sortingOrder = 30;
            self.View.SpriteRenderer.color = new Color(1f, 0.85f, 0.1f, 1f);
            self.CachedTransform.localScale = new Vector3(VisualScale, VisualScale, 1f);
            self.PresentationPosition.Reset();
            self.CachedTransform.position = self.PresentationPosition.SetTarget(new Vector3(self.Entry.State.PositionX / 1000f, self.Entry.State.PositionY / 1000f, 0f));
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(this SurvivorProjectileUGFEntity self, float elapseSeconds, float realElapseSeconds)
        {
            self.ObserveChanges();
            self.CachedTransform.position = self.PresentationPosition.Advance(realElapseSeconds);
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorProjectileUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.PresentationPosition.Reset();
            self.Entry = null;
        }

        [ETReactiveBind(nameof(SurvivorProjectileUGFEntity.PositionX), nameof(SurvivorProjectileUGFEntity.PositionY))]
        private static void OnPositionChanged(this SurvivorProjectileUGFEntity self, int positionX, int positionY)
        {
            self.CachedTransform.position = self.PresentationPosition.SetTarget(new Vector3(positionX / 1000f, positionY / 1000f, 0f));
        }
    }
}
