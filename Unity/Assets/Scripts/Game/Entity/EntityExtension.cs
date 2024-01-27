using System;
using UnityGameFramework.Runtime;

namespace Game
{
    public static partial class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId = 0;

        public static void HideEntity(this EntityComponent entityComponent, AEntity aEntity)
        {
            entityComponent.HideEntity(aEntity.Entity);
        }

        public static void TryHideEntity(this EntityComponent entityComponent, int serialId)
        {
            if (entityComponent.HasEntity(serialId))
            {
                entityComponent.HideEntity(serialId);
            }
        }

        public static int? ShowEntity<T>(this EntityComponent entityComponent, int entityTypeId, object userData = null) where T : EntityLogic
        {
            return entityComponent.ShowEntity(entityTypeId, typeof (T), userData);
        }

        public static int? ShowEntity(this EntityComponent entityComponent, int entityTypeId, Type logicType, object userData = null)
        {
            DREntity drEntity = GameEntry.Tables.DTEntity.GetOrDefault(entityTypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", entityTypeId.ToString());
                return null;
            }

            int entityId = entityComponent.GenerateSerialId();
            entityComponent.ShowEntity(entityId, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), drEntity.EntityGroupName, drEntity.Priority, userData);
            return entityId;
        }

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }
    }
}
