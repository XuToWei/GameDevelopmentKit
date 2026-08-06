using TMPro;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorDamageNumberUGFEntity))]
    public static partial class SurvivorDamageNumberUGFEntitySystem
    {
        private const float LifetimeSeconds = 0.8f;
        private const float FadeStartSeconds = 0.42f;
        private const float RiseWorldUnitsPerSecond = 0.8f;
        private const float StartWorldY = 0.68f;

        [EntitySystem]
        private static void Awake(this SurvivorDamageNumberUGFEntity self)
        {
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorDamageNumberUGFEntity self)
        {
            SurvivorDamageNumberEntry entry = self.GetParent<SurvivorDamageNumberEntry>();
            self.ElapsedSeconds = 0f;
            self.View.SpriteRenderer.enabled = false;
            self.View.FillSpriteRenderer.enabled = false;

            TextMeshPro damageText = self.View.DamageTextTextMeshPro;
            damageText.enabled = true;
            if (damageText.font == null)
            {
                damageText.font = TMP_Settings.defaultFontAsset;
            }

            damageText.text = $"-{entry.Damage}";
            damageText.alignment = TextAlignmentOptions.Center;
            damageText.fontStyle = FontStyles.Bold;
            damageText.fontSize = 36f;
            damageText.color = new Color(1f, 0.74f, 0.18f, 1f);
            damageText.sortingOrder = 60;
            self.View.DamageTextRectTransform.localPosition = Vector3.zero;
            self.View.DamageTextRectTransform.localScale = Vector3.one * 0.14f;

            int horizontalSlot = (int)(entry.Id % 3) - 1;
            self.CachedTransform.position = new Vector3(
                entry.PositionX + horizontalSlot * 0.18f,
                entry.PositionY + StartWorldY,
                -0.02f);
            self.CachedTransform.rotation = Quaternion.identity;
            self.CachedTransform.localScale = Vector3.one * 0.72f;
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(
            this SurvivorDamageNumberUGFEntity self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            float deltaTime = Mathf.Max(0f, realElapseSeconds);
            self.ElapsedSeconds += deltaTime;
            self.CachedTransform.position += Vector3.up * RiseWorldUnitsPerSecond * deltaTime;
            float lifetimeRatio = Mathf.Clamp01(self.ElapsedSeconds / LifetimeSeconds);
            float scale = lifetimeRatio < 0.18f
                    ? Mathf.Lerp(0.72f, 1.08f, lifetimeRatio / 0.18f)
                    : Mathf.Lerp(1.08f, 0.96f, (lifetimeRatio - 0.18f) / 0.82f);
            self.CachedTransform.localScale = Vector3.one * scale;

            Color color = self.View.DamageTextTextMeshPro.color;
            color.a = self.ElapsedSeconds <= FadeStartSeconds
                    ? 1f
                    : 1f - Mathf.InverseLerp(
                        FadeStartSeconds,
                        LifetimeSeconds,
                        self.ElapsedSeconds);
            self.View.DamageTextTextMeshPro.color = color;
            if (self.ElapsedSeconds < LifetimeSeconds)
            {
                return;
            }

            self.GetParent<SurvivorDamageNumberEntry>().Dispose();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorDamageNumberUGFEntity self, bool isShutdown)
        {
            self.View.DamageTextTextMeshPro.enabled = false;
            self.View.SpriteRenderer.enabled = true;
            self.View.FillSpriteRenderer.enabled = false;
            self.CachedTransform.localScale = Vector3.one;
            self.ElapsedSeconds = 0f;
        }
    }
}
