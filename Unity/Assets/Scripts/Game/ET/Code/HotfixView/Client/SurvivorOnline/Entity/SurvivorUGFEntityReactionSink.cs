using UnityEngine;

namespace ET.Client
{
    [EnableClass]
    public sealed class SurvivorUGFEntityReactionSink: ISurvivorUGFEntityReactionSink
    {
        public void OnMonsterPositionChanged(
            SurvivorMonsterUGFEntity entity,
            int positionX,
            int positionY)
        {
            entity.CachedTransform.position = new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }

        public void OnProjectilePositionChanged(
            SurvivorProjectileUGFEntity entity,
            int positionX,
            int positionY)
        {
            entity.CachedTransform.position = new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }

        public void OnPickupPositionChanged(
            SurvivorPickupUGFEntity entity,
            int positionX,
            int positionY)
        {
            entity.CachedTransform.position = new Vector3(positionX / 1000f, positionY / 1000f, 0f);
        }
    }
}
