using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game;
using GameEntry = Game.GameEntry;

namespace ET.Client
{
    [EntitySystemOf(typeof(UGFEntityComponent))]
    [FriendOf(typeof(UGFEntityComponent))]
    public static partial class UGFEntityComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UGFEntityComponent self)
        {
            self.AllShowEntities.Clear();
        }

        [EntitySystem]
        private static void Destroy(this UGFEntityComponent self)
        {
            self.HideAllEntities();
        }

        public static async UniTask<UGFEntity> ShowEntityAsync<T>(this UGFEntityComponent self, int entityId, object userData = null) where T : IUGFEntityEvent
        {
            ETMonoEntityData entityData = ETMonoEntityData.Acquire(typeof(T), self, userData);
            UnityGameFramework.Runtime.Entity entity = await GameEntry.Entity.ShowEntityAsync<ETMonoEntity>(entityId, entityData);
            if (entity == null)
            {
                entityData.Release();
                return null;
            }
            if (entity.Logic is not ETMonoEntity etMonoEntity)
            {
                throw new Exception($"Show Entity fail! EntityId:{entityId}) is not ETMonoEntity!");
            }
            return etMonoEntity.UGFEntity;
        }

        public static void HideEntity(this UGFEntityComponent self, UGFEntity entity)
        {
            if (!self.AllShowEntities.Contains(entity))
                return;
            GameEntry.Entity.HideEntity(entity.Entity);
        }

        public static void HideEntity<T>(this UGFEntityComponent self) where T : IUGFEntityEvent
        {
            using HashSetComponent<UGFEntity> needRemoves = new HashSetComponent<UGFEntity>();
            long eventTypeLongHashCode = typeof(T).FullName.GetLongHashCode();
            foreach (UGFEntity entity in self.AllShowEntities)
            {
                if (entity.EntityEventTypeLongHashCode == eventTypeLongHashCode)
                {
                    needRemoves.Add(entity);
                }
            }
            foreach (UGFEntity entity in needRemoves)
            {
                self.HideEntity(entity);
            }
        }

        public static void HideAllEntities(this UGFEntityComponent self)
        {
            foreach (UGFEntity entity in self.AllShowEntities.ToArray())
            {
                if (entity != null)
                {
                    self.HideEntity(entity);
                }
            }
            self.AllShowEntities.Clear();
        }
    }
}