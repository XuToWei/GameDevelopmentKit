using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorHealthBarUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorHealthBarUGFEntitySystem
    {
        private const float PlayerBarWidth = 1.15f;
        private const float PlayerBarVerticalOffset = -0.66f;
        private const float MonsterBarWidth = 0.95f;
        private const float MonsterBarVerticalOffset = -0.59f;
        private const float BarHeight = 0.18f;
        private const float FillWidthRatio = 0.93f;
        private const float FillHeightRatio = 0.56f;

        [EntitySystem]
        private static void Awake(this SurvivorHealthBarUGFEntity self, bool isPlayer)
        {
            self.IsPlayer = isPlayer;
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorHealthBarUGFEntity self)
        {
            UGFEntity ownerEntity;
            float barWidth;
            float verticalOffset;
            if (self.IsPlayer)
            {
                SurvivorPlayerEntry entry = self.GetParent<SurvivorPlayerEntry>();
                SurvivorClientComponent client = entry.GetParent<SurvivorClientComponent>();
                self.PlayerState = client.GetComponent<SurvivorViewEntityManagerComponent>()
                        .Runtime.PlayerStates[entry.Id];
                ownerEntity = entry.GetComponent<SurvivorPlayerUGFEntity>();
                barWidth = PlayerBarWidth;
                verticalOffset = PlayerBarVerticalOffset;
            }
            else
            {
                SurvivorMonsterEntry entry = self.GetParent<SurvivorMonsterEntry>();
                SurvivorClientComponent client = entry.GetParent<SurvivorClientComponent>();
                self.MonsterState = client.GetComponent<SurvivorViewEntityManagerComponent>()
                        .Runtime.MonsterStates[entry.Id];
                ownerEntity = entry.GetComponent<SurvivorMonsterUGFEntity>();
                barWidth = MonsterBarWidth;
                verticalOffset = MonsterBarVerticalOffset;
            }

            self.OwnerEntity = ownerEntity;
            self.VerticalOffset = verticalOffset;
            self.View.SpriteRenderer.enabled = true;
            self.View.SpriteRenderer.color = new Color(0.015f, 0.025f, 0.035f, 0.94f);
            self.View.SpriteRenderer.sortingOrder = 45;
            SpriteRenderer fillRenderer = self.View.FillSpriteRenderer;
            fillRenderer.sprite = self.View.SpriteRenderer.sprite;
            fillRenderer.sharedMaterial = self.View.SpriteRenderer.sharedMaterial;
            fillRenderer.enabled = true;
            fillRenderer.sortingOrder = 46;
            self.View.DamageTextTextMeshPro.enabled = false;

            Sprite sprite = self.View.SpriteRenderer.sprite;
            float spriteWidth = sprite == null ? 1f : sprite.bounds.size.x;
            float spriteHeight = sprite == null ? 1f : sprite.bounds.size.y;
            self.CachedTransform.rotation = Quaternion.identity;
            self.CachedTransform.localScale = new Vector3(
                barWidth / spriteWidth,
                BarHeight / spriteHeight,
                1f);
            self.UpdateWorldPosition();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorHealthBarUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.ObserveChanges();
            self.UpdateWorldPosition();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorHealthBarUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.View.FillSpriteRenderer.enabled = false;
            self.View.DamageTextTextMeshPro.enabled = false;
            self.View.SpriteRenderer.enabled = true;
            self.PlayerState = null;
            self.MonsterState = null;
            self.OwnerEntity = null;
            self.VerticalOffset = 0f;
        }

        private static void UpdateWorldPosition(this SurvivorHealthBarUGFEntity self)
        {
            if (self.OwnerEntity == null || self.OwnerEntity.CachedTransform == null)
            {
                return;
            }

            Vector3 ownerPosition = self.OwnerEntity.CachedTransform.position;
            self.CachedTransform.position = new Vector3(
                ownerPosition.x,
                ownerPosition.y + self.VerticalOffset,
                -0.01f);
        }

        [ETReactiveBind(nameof(SurvivorHealthBarUGFEntity.Hp), nameof(SurvivorHealthBarUGFEntity.MaxHp))]
        private static void OnHealthChanged(
            this SurvivorHealthBarUGFEntity self,
            int oldHp,
            int hp,
            int oldMaxHp,
            int maxHp)
        {
            float ratio = maxHp <= 0 ? 0f : Mathf.Clamp01(hp / (float)maxHp);
            SpriteRenderer fillRenderer = self.View.FillSpriteRenderer;
            fillRenderer.transform.localScale = new Vector3(
                FillWidthRatio * ratio,
                FillHeightRatio,
                1f);
            Sprite sprite = fillRenderer.sprite;
            float spriteWidth = sprite == null ? 1f : sprite.bounds.size.x;
            fillRenderer.transform.localPosition = new Vector3(
                -spriteWidth * (1f - FillWidthRatio * ratio) * 0.5f,
                0f,
                -0.01f);
            fillRenderer.color = ratio >= 0.5f
                    ? Color.Lerp(
                        new Color(1f, 0.78f, 0.08f, 1f),
                        new Color(0.12f, 0.95f, 0.32f, 1f),
                        (ratio - 0.5f) * 2f)
                    : Color.Lerp(
                        new Color(1f, 0.16f, 0.1f, 1f),
                        new Color(1f, 0.78f, 0.08f, 1f),
                        ratio * 2f);
            if (hp >= oldHp)
            {
                return;
            }

            SurvivorClientComponent client;
            Vector3 position;
            if (self.IsPlayer)
            {
                SurvivorPlayerEntry entry = self.GetParent<SurvivorPlayerEntry>();
                client = entry.GetParent<SurvivorClientComponent>();
                position = entry.GetComponent<SurvivorPlayerUGFEntity>().CachedTransform.position;
            }
            else
            {
                SurvivorMonsterEntry entry = self.GetParent<SurvivorMonsterEntry>();
                client = entry.GetParent<SurvivorClientComponent>();
                position = entry.GetComponent<SurvivorMonsterUGFEntity>().CachedTransform.position;
            }

            client.GetComponent<SurvivorViewEntityManagerComponent>()
                    .CreateDamageNumber(oldHp - hp, position.x, position.y);
        }
    }
}
