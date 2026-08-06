using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorPlayerUGFEntity))]
    [ETReactiveSystem]
    public static partial class SurvivorPlayerUGFEntitySystem
    {
        private const float VisualScale = 7f;
        private const float SwordWaveVisualDurationSeconds = 0.18f;

        [EntitySystem]
        private static void Awake(this SurvivorPlayerUGFEntity self)
        {
            self.PresentationPosition = new SurvivorPresentationPosition();
        }

        [UGFEntitySystem]
        private static void UGFEntityOnShow(this SurvivorPlayerUGFEntity self)
        {
            self.Entry = self.GetParent<SurvivorPlayerEntry>();
            SurvivorClientComponent client = self.Entry.GetParent<SurvivorClientComponent>();
            self.IsLocalPlayer = self.Entry.State.PlayerId == client.PlayerId;
            self.View.SpriteRenderer.color = new Color(0.12f, 0.72f, 1f, 1f);
            self.View.SpriteRenderer.sortingOrder = 20;
            self.CachedTransform.localScale = new Vector3(VisualScale, VisualScale, 1f);
            self.EnsureSwordWaveVisual();
            self.SwordWaveVisualRemainingSeconds = 0f;
            self.SwordWaveVisual.SetActive(false);
            self.PresentationPosition.Reset();
            if (self.IsLocalPlayer)
            {
                client.EnsureLocalPredictionInitialized();
                self.CachedTransform.position = new Vector3(
                    client.LocalPrediction.PresentationPositionX,
                    client.LocalPrediction.PresentationPositionY,
                    0f);
            }
        }

        [UGFEntitySystem]
        private static void UGFEntityOnUpdate(this SurvivorPlayerUGFEntity self, float elapseSeconds, float realElapseSeconds)
        {
            self.ObserveChanges();
            self.UpdateSwordWaveVisual(realElapseSeconds);
            if (self.IsLocalPlayer)
            {
                SurvivorLocalPlayerPrediction prediction = self.Entry.GetParent<SurvivorClientComponent>().LocalPrediction;
                self.CachedTransform.position = new Vector3(prediction.PresentationPositionX, prediction.PresentationPositionY, 0f);
                return;
            }

            self.CachedTransform.position = self.PresentationPosition.Advance(realElapseSeconds);
        }

        [UGFEntitySystem]
        private static void UGFEntityOnHide(this SurvivorPlayerUGFEntity self, bool isShutdown)
        {
            self.ClearReactive();
            self.SwordWaveVisualRemainingSeconds = 0f;
            self.SwordWaveVisual.SetActive(false);
            self.PresentationPosition.Reset();
            self.IsLocalPlayer = false;
            self.Entry = null;
        }

        [ETReactiveBind(nameof(SurvivorPlayerUGFEntity.SwordWaveRevision))]
        private static void OnSwordWaveTriggered(this SurvivorPlayerUGFEntity self, long swordWaveRevision)
        {
            if (swordWaveRevision <= 0)
            {
                return;
            }

            self.EnsureSwordWaveVisual();
            self.SwordWaveVisualRemainingSeconds = SwordWaveVisualDurationSeconds;
            self.SwordWaveVisual.SetActive(true);
        }

        [ETReactiveBind(nameof(SurvivorPlayerUGFEntity.PositionX), nameof(SurvivorPlayerUGFEntity.PositionY))]
        private static void OnPositionChanged(this SurvivorPlayerUGFEntity self, int positionX, int positionY)
        {
            if (self.IsLocalPlayer)
            {
                return;
            }

            self.CachedTransform.position = self.PresentationPosition.SetTarget(new Vector3(positionX / 1000f, positionY / 1000f, 0f));
        }

        private static void EnsureSwordWaveVisual(this SurvivorPlayerUGFEntity self)
        {
            Sprite sprite = self.View.SpriteRenderer.sprite;
            if (self.SwordWaveVisual == null)
            {
                self.SwordWaveVisual = new GameObject("Sword Wave Range");
                self.SwordWaveVisual.transform.SetParent(self.CachedTransform, false);
                self.SwordWaveRenderer = self.SwordWaveVisual.AddComponent<SpriteRenderer>();
            }

            self.SwordWaveRenderer.sprite = sprite;
            self.SwordWaveRenderer.sharedMaterial = self.View.SpriteRenderer.sharedMaterial;
            self.SwordWaveRenderer.color = new Color(0.2f, 0.9f, 1f, 0.72f);
            self.SwordWaveRenderer.sortingOrder = 25;
            self.SwordWaveVisual.transform.localPosition = Vector3.zero;
            float spriteWidth = sprite == null ? 1f : sprite.bounds.size.x;
            float spriteHeight = sprite == null ? 1f : sprite.bounds.size.y;
            float rootScaleX = Mathf.Max(Mathf.Abs(self.CachedTransform.lossyScale.x), 0.001f);
            float rootScaleY = Mathf.Max(Mathf.Abs(self.CachedTransform.lossyScale.y), 0.001f);
            self.SwordWaveVisual.transform.localScale = new Vector3(
                SurvivorDefaults.SwordWaveRangeX * 2f / 1000f / spriteWidth / rootScaleX,
                SurvivorDefaults.SwordWaveRangeY * 2f / 1000f / spriteHeight / rootScaleY,
                1f);
        }

        private static void UpdateSwordWaveVisual(
            this SurvivorPlayerUGFEntity self,
            float realElapseSeconds)
        {
            if (self.SwordWaveVisualRemainingSeconds <= 0f)
            {
                return;
            }

            self.SwordWaveVisualRemainingSeconds -= Mathf.Max(0f, realElapseSeconds);
            if (self.SwordWaveVisualRemainingSeconds <= 0f)
            {
                self.SwordWaveVisual.SetActive(false);
            }
        }
    }
}
