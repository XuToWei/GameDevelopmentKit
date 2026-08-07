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
            self.Prediction = new SurvivorProjectilePrediction();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorProjectileUGFEntity self)
        {
            self.Entry = self.GetParent<SurvivorProjectileEntry>();
            self.View.SpriteRenderer.sortingOrder = 30;
            self.View.SpriteRenderer.color = new Color(1f, 0.85f, 0.1f, 1f);
            self.CachedTransform.localScale = new Vector3(VisualScale, VisualScale, 1f);
            self.Prediction.Reset();
            self.CachedTransform.position = self.Prediction.Initialize(
                self.Entry.State.PositionX,
                self.Entry.State.PositionY,
                self.Entry.State.VelocityX,
                self.Entry.State.VelocityY);
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(this SurvivorProjectileUGFEntity self, float elapseSeconds, float realElapseSeconds)
        {
            self.ObserveChanges();
            self.CachedTransform.position = self.Prediction.Advance(realElapseSeconds);
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorProjectileUGFEntity self, bool isShutdown)
        {
            self.ResetReactive();
            self.Prediction.Reset();
            self.Entry = null;
        }

        [ETReactiveBind(
            nameof(SurvivorProjectileUGFEntity.PositionX),
            nameof(SurvivorProjectileUGFEntity.PositionY),
            nameof(SurvivorProjectileUGFEntity.VelocityX),
            nameof(SurvivorProjectileUGFEntity.VelocityY))]
        private static void OnStateChanged(
            this SurvivorProjectileUGFEntity self,
            int positionX,
            int positionY,
            int velocityX,
            int velocityY)
        {
            self.CachedTransform.position = self.Prediction.Reconcile(
                positionX,
                positionY,
                velocityX,
                velocityY);
        }
    }
}
