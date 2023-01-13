using System;
using Game;
using GameEntry = Game.GameEntry;

namespace ET.Client
{
    [FriendOf(typeof(EntityComponent))]
    [FriendOf(typeof(UGFEntity))]
    public static class EntityComponentSystem
    {
        [ObjectSystem]
        public class EntityComponentAwakeSystem: AwakeSystem<EntityComponent>
        {
            protected override void Awake(EntityComponent self)
            {
                EntityComponent.Instance = self;
            }
        }

        [ObjectSystem]
        public class EntityComponentDestroySystem: DestroySystem<EntityComponent>
        {
            protected override void Destroy(EntityComponent self)
            {
                EntityComponent.Instance = null;
            }
        }
        
        public static async ETTask<UGFEntity> ShowEntityAsync<T>(this EntityComponent self, int entityTypeId, object userData = null) where T : AUGFEntityEvent
        {
            ETMonoEntityData formData = ETMonoEntityData.Acquire(typeof (T), self, userData);
            UnityGameFramework.Runtime.Entity entity = await GameEntry.Entity.ShowEntityAsync<ETMonoEntity>(entityTypeId, formData);
            if (entity == null)
                return null;
            formData.Release();
            return (entity.Logic as ETMonoEntity).UGFEntity;
        }

        public static void HideEntity(this EntityComponent self, UGFEntity entity)
        {
            GameEntry.Entity.HideEntity(entity.etMonoEntity);
        }

        public static void HideEntity(this UIComponent self, int entityId)
        {
            GameEntry.Entity.HideEntity(entityId);
        }
        
        public static UGFEntity GetEntity(this EntityComponent self, int entityId)
        {
            UnityGameFramework.Runtime.Entity entity = GameEntry.Entity.GetEntity(entityId);
            if (entity == null)
            {
                return null;
            }

            if (entity.Logic is not ETMonoEntity etMonoEntity)
            {
                throw new Exception($"Get UGFEntity fail! entityId:{entityId}) is not ETMonoEntity!");
            }

            return etMonoEntity.UGFEntity;
        }

        public static void AttachEntity(this EntityComponent self, UGFEntity childEntity, UGFEntity parentEntity, string parentTransformPath, object userData = null)
        {
            GameEntry.Entity.AttachEntity(childEntity.etMonoEntity.Entity, parentEntity.etMonoEntity.Entity, parentTransformPath, userData);
        }

        public static void DetachEntity(this EntityComponent self, UGFEntity childEntity, object userData = null)
        {
            GameEntry.Entity.DetachEntity(childEntity.etMonoEntity.Entity, userData);
        }
        
        public static void DetachChildEntities(this EntityComponent self, UGFEntity parentEntity, object userData = null)
        {
            GameEntry.Entity.DetachChildEntities(parentEntity.etMonoEntity.Entity, userData);
        }
    }
}
