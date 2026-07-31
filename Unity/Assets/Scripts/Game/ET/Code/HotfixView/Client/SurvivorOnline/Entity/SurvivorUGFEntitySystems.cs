using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorPlayerUGFEntity))]
    public static partial class SurvivorPlayerUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorPlayerUGFEntity self)
        {
            self.Observer = SurvivorPlayerUGFEntityReactiveObserver.Create(self);
            self.Observer.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorPlayerUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.Observer.ObserveChanges();
            self.Observer.ResetChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorPlayerUGFEntity self, bool isShutdown)
        {
            SurvivorPlayerUGFEntityReactiveObserver.Recycle(self.Observer);
            self.Observer = null;
        }
    }

    [EntitySystemOf(typeof(SurvivorMonsterUGFEntity))]
    public static partial class SurvivorMonsterUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorMonsterUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorMonsterUGFEntity self)
        {
            self.View.SpriteRenderer.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            self.View.SpriteRenderer.sortingOrder = 10;
            self.CachedTransform.localScale = new Vector3(0.8f, 0.8f, 1f);
            self.Observer = new SurvivorMonsterUGFEntityReactiveObserver(
                self,
                self.GetParent<SurvivorMonsterEntry>()
                        .GetParent<SurvivorClientComponent>()
                        .Runtime
                        .MonsterStates[self.GetParent<SurvivorMonsterEntry>().Id],
                new SurvivorUGFEntityReactionSink());
            self.Observer.ResetChanges();
            self.GetParent<SurvivorMonsterEntry>()
                    .GetParent<SurvivorClientComponent>()
                    .RegisterPresentationObserver(self.Observer);
            self.Observer.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorMonsterUGFEntity self, bool isShutdown)
        {
            self.GetParent<SurvivorMonsterEntry>()
                    .GetParent<SurvivorClientComponent>()
                    .UnregisterPresentationObserver(self.Observer);
            self.Observer.ResetChanges();
            self.Observer = null;
        }
    }

    [EntitySystemOf(typeof(SurvivorProjectileUGFEntity))]
    public static partial class SurvivorProjectileUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorProjectileUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorProjectileUGFEntity self)
        {
            self.View.SpriteRenderer.color = new Color(1f, 0.85f, 0.1f, 1f);
            self.View.SpriteRenderer.sortingOrder = 30;
            self.CachedTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
            self.Observer = new SurvivorProjectileUGFEntityReactiveObserver(
                self,
                self.GetParent<SurvivorProjectileEntry>()
                        .GetParent<SurvivorClientComponent>()
                        .Runtime
                        .ProjectileStates[self.GetParent<SurvivorProjectileEntry>().Id],
                new SurvivorUGFEntityReactionSink());
            self.Observer.ResetChanges();
            self.GetParent<SurvivorProjectileEntry>()
                    .GetParent<SurvivorClientComponent>()
                    .RegisterPresentationObserver(self.Observer);
            self.Observer.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorProjectileUGFEntity self, bool isShutdown)
        {
            self.GetParent<SurvivorProjectileEntry>()
                    .GetParent<SurvivorClientComponent>()
                    .UnregisterPresentationObserver(self.Observer);
            self.Observer.ResetChanges();
            self.Observer = null;
        }
    }

    [EntitySystemOf(typeof(SurvivorPickupUGFEntity))]
    public static partial class SurvivorPickupUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPickupUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorPickupUGFEntity self)
        {
            self.View.SpriteRenderer.color = new Color(0.25f, 1f, 0.35f, 1f);
            self.View.SpriteRenderer.sortingOrder = 5;
            self.CachedTransform.localScale = new Vector3(0.4f, 0.4f, 1f);
            self.Observer = new SurvivorPickupUGFEntityReactiveObserver(
                self,
                self.GetParent<SurvivorPickupEntry>()
                        .GetParent<SurvivorClientComponent>()
                        .Runtime
                        .PickupStates[self.GetParent<SurvivorPickupEntry>().Id],
                new SurvivorUGFEntityReactionSink());
            self.Observer.ResetChanges();
            self.GetParent<SurvivorPickupEntry>()
                    .GetParent<SurvivorClientComponent>()
                    .RegisterPresentationObserver(self.Observer);
            self.Observer.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorPickupUGFEntity self, bool isShutdown)
        {
            self.GetParent<SurvivorPickupEntry>()
                    .GetParent<SurvivorClientComponent>()
                    .UnregisterPresentationObserver(self.Observer);
            self.Observer.ResetChanges();
            self.Observer = null;
        }
    }
}
