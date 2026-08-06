using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorPickupUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorPickupUGFEntitySystem
    {
        private const float VisualScale = 4.375f;

        [EntitySystem]
        private static void Awake(this SurvivorPickupUGFEntity self)
        {
            self.PresentationPosition = new SurvivorPresentationPosition();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorPickupUGFEntity self)
        {
            SurvivorPickupEntry entry = self.GetParent<SurvivorPickupEntry>();
            self.State = entry
                    .GetParent<SurvivorClientComponent>()
                    .GetComponent<SurvivorViewEntityManagerComponent>()
                    .Runtime
                    .PickupStates[entry.Id];
            self.View.SpriteRenderer.enabled = true;
            self.View.FillSpriteRenderer.enabled = false;
            self.View.DamageTextTextMeshPro.enabled = false;
            self.View.SpriteRenderer.color = new Color(0.25f, 1f, 0.35f, 1f);
            self.View.SpriteRenderer.sortingOrder = 5;
            self.CachedTransform.localScale = new Vector3(VisualScale, VisualScale, 1f);
            self.PresentationPosition.Reset();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorPickupUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.ObserveChanges();
            self.CachedTransform.position = self.PresentationPosition.Advance(realElapseSeconds);
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorPickupUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.PresentationPosition.Reset();
            self.State = null;
        }

        [ETReactiveBind(nameof(SurvivorPickupUGFEntity.PositionX), nameof(SurvivorPickupUGFEntity.PositionY))]
        private static void OnPositionChanged(
            this SurvivorPickupUGFEntity self,
            int positionX,
            int positionY)
        {
            self.CachedTransform.position = self.PresentationPosition.SetTarget(
                new Vector3(positionX / 1000f, positionY / 1000f, 0f));
        }
    }
}
