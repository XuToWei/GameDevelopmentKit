using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [FriendOf(typeof(EntityComponent))]
    [FriendOf(typeof(UGFEntity))]
    public static class EntityComponentSystem
    {
        [EntitySystem]
        private class EntityComponentAwakeSystem: AwakeSystem<EntityComponent>
        {
            protected override void Awake(EntityComponent self)
            {
                self.AllShowEntities.Clear();
            }
        }

        [EntitySystem]
        private class EntityComponentDestroySystem: DestroySystem<EntityComponent>
        {
            protected override void Destroy(EntityComponent self)
            {
                self.HideAllEntities();
            }
        }
        
        public static async UniTask<UGFEntity> ShowEntityAsync<T>(this EntityComponent self, int entityTypeId, object userData = null) where T : AUGFEntityEvent
        {
            ETMonoEntityData formData = ETMonoEntityData.Acquire(typeof (T), self, userData);
            UnityGameFramework.Runtime.Entity entity = await GameEntry.Entity.ShowEntityAsync<ETMonoEntity>(entityTypeId, formData);
            if (entity == null)
            {
                formData.Release();
                return null;
            }
            if (entity.Logic is not ETMonoEntity etMonoEntity)
            {
                throw new Exception($"Get UGFEntity fail! EntityTypeId:{entityTypeId}) is not ETMonoEntity!");
            }
            return etMonoEntity.ugfEntity;
        }

        public static void HideEntity(this EntityComponent self, UGFEntity entity)
        {
            GameEntry.Entity.HideEntity(entity.entity);
        }

        public static void HideEntity(this EntityComponent self, int entityId)
        {
            GameEntry.Entity.HideEntity(entityId);
        }
        
        public static void HideAllEntities(this EntityComponent self)
        {
            foreach (UGFEntity entity in self.AllShowEntities.ToArray())
            {
                self.HideEntity(entity);
            }
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

            return etMonoEntity.ugfEntity;
        }

        public static void AttachEntity(this EntityComponent self, UGFEntity childEntity, UGFEntity parentEntity, string parentTransformPath, object userData = null)
        {
            GameEntry.Entity.AttachEntity(childEntity.entity, parentEntity.entity, parentTransformPath, userData);
        }

        public static void DetachEntity(this EntityComponent self, UGFEntity childEntity, object userData = null)
        {
            GameEntry.Entity.DetachEntity(childEntity.entity, userData);
        }
        
        public static void DetachChildEntities(this EntityComponent self, UGFEntity parentEntity, object userData = null)
        {
            GameEntry.Entity.DetachChildEntities(parentEntity.entity, userData);
        }
    }
}
