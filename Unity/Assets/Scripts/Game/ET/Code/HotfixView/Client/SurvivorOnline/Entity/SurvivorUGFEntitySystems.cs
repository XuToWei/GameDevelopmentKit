using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorPlayerUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorPlayerUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorPlayerUGFEntity self)
        {
            SurvivorPlayerEntry entry = self.GetParent<SurvivorPlayerEntry>();
            self.State = entry
                    .GetParent<SurvivorClientComponent>()
                    .Runtime
                    .PlayerStates[entry.Id];
            self.View.SpriteRenderer.color = new Color(0.2f, 0.55f, 1f, 1f);
            self.View.SpriteRenderer.sortingOrder = 20;
            self.CachedTransform.localScale = Vector3.one;
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorPlayerUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorPlayerUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.State = null;
        }

        [ETReactiveSource]
        private static int PositionX(this SurvivorPlayerUGFEntity self)
        {
            return self.State.PositionX;
        }

        [ETReactiveSource]
        private static int PositionY(this SurvivorPlayerUGFEntity self)
        {
            return self.State.PositionY;
        }

        [ETReactiveBind(nameof(PositionX), nameof(PositionY))]
        private static void OnPositionChanged(
            this SurvivorPlayerUGFEntity self,
            int positionX,
            int positionY)
        {
            self.CachedTransform.position = new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }
    }

    [EntitySystemOf(typeof(SurvivorMonsterUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorMonsterUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorMonsterUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorMonsterUGFEntity self)
        {
            SurvivorMonsterEntry entry = self.GetParent<SurvivorMonsterEntry>();
            self.State = entry
                    .GetParent<SurvivorClientComponent>()
                    .Runtime
                    .MonsterStates[entry.Id];
            self.View.SpriteRenderer.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            self.View.SpriteRenderer.sortingOrder = 10;
            self.CachedTransform.localScale = new Vector3(0.8f, 0.8f, 1f);
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorMonsterUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorMonsterUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.State = null;
        }

        [ETReactiveSource]
        private static int PositionX(this SurvivorMonsterUGFEntity self)
        {
            return self.State.PositionX;
        }

        [ETReactiveSource]
        private static int PositionY(this SurvivorMonsterUGFEntity self)
        {
            return self.State.PositionY;
        }

        [ETReactiveBind(nameof(PositionX), nameof(PositionY))]
        private static void OnPositionChanged(
            this SurvivorMonsterUGFEntity self,
            int positionX,
            int positionY)
        {
            self.CachedTransform.position = new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }
    }

    [EntitySystemOf(typeof(SurvivorProjectileUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorProjectileUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorProjectileUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorProjectileUGFEntity self)
        {
            SurvivorProjectileEntry entry = self.GetParent<SurvivorProjectileEntry>();
            self.State = entry
                    .GetParent<SurvivorClientComponent>()
                    .Runtime
                    .ProjectileStates[entry.Id];
            self.View.SpriteRenderer.color = new Color(1f, 0.85f, 0.1f, 1f);
            self.View.SpriteRenderer.sortingOrder = 30;
            self.CachedTransform.localScale = new Vector3(0.3f, 0.3f, 1f);
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorProjectileUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorProjectileUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.State = null;
        }

        [ETReactiveSource]
        private static int PositionX(this SurvivorProjectileUGFEntity self)
        {
            return self.State.PositionX;
        }

        [ETReactiveSource]
        private static int PositionY(this SurvivorProjectileUGFEntity self)
        {
            return self.State.PositionY;
        }

        [ETReactiveBind(nameof(PositionX), nameof(PositionY))]
        private static void OnPositionChanged(
            this SurvivorProjectileUGFEntity self,
            int positionX,
            int positionY)
        {
            self.CachedTransform.position = new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }
    }

    [EntitySystemOf(typeof(SurvivorPickupUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorPickupUGFEntitySystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPickupUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorPickupUGFEntity self)
        {
            SurvivorPickupEntry entry = self.GetParent<SurvivorPickupEntry>();
            self.State = entry
                    .GetParent<SurvivorClientComponent>()
                    .Runtime
                    .PickupStates[entry.Id];
            self.View.SpriteRenderer.color = new Color(0.25f, 1f, 0.35f, 1f);
            self.View.SpriteRenderer.sortingOrder = 5;
            self.CachedTransform.localScale = new Vector3(0.4f, 0.4f, 1f);
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorPickupUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.ObserveChanges();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorPickupUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.State = null;
        }

        [ETReactiveSource]
        private static int PositionX(this SurvivorPickupUGFEntity self)
        {
            return self.State.PositionX;
        }

        [ETReactiveSource]
        private static int PositionY(this SurvivorPickupUGFEntity self)
        {
            return self.State.PositionY;
        }

        [ETReactiveBind(nameof(PositionX), nameof(PositionY))]
        private static void OnPositionChanged(
            this SurvivorPickupUGFEntity self,
            int positionX,
            int positionY)
        {
            self.CachedTransform.position = new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }
    }
}
