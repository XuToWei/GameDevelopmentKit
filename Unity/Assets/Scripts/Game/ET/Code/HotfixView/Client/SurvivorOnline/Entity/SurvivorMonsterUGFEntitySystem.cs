using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorMonsterUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorMonsterUGFEntitySystem
    {
        private const float VisualScale = 6.25f;

        [EntitySystem]
        private static void Awake(this SurvivorMonsterUGFEntity self)
        {
            self.PresentationPosition = new SurvivorPresentationPosition();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorMonsterUGFEntity self)
        {
            self.Entry = self.GetParent<SurvivorMonsterEntry>();
            self.View.SpriteRenderer.color = new Color(1f, 0.22f, 0.18f, 1f);
            self.View.SpriteRenderer.sortingOrder = 10;
            self.CachedTransform.localScale = new Vector3(VisualScale, VisualScale, 1f);
            self.PresentationPosition.Reset();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(this SurvivorMonsterUGFEntity self, float elapseSeconds, float realElapseSeconds)
        {
            self.ObserveChanges();
            self.CachedTransform.position = self.PresentationPosition.Advance(realElapseSeconds);
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorMonsterUGFEntity self, bool isShutdown)
        {
            self.ResetReactive();
            self.PresentationPosition.Reset();
            self.Entry = null;
        }

        [ETReactiveBind(nameof(SurvivorMonsterUGFEntity.PositionX), nameof(SurvivorMonsterUGFEntity.PositionY))]
        private static void OnPositionChanged(this SurvivorMonsterUGFEntity self, int positionX, int positionY)
        {
            self.CachedTransform.position = self.PresentationPosition.SetTarget(new Vector3(positionX / 1000f, positionY / 1000f, 0f));
        }
    }
}
