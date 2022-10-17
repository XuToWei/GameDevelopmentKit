//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using cfg;
using cfg.UGF;
using UnityGameFramework.Runtime;

namespace UGF
{
    public static class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId { get; set; } = 0;

        public static UGFEntity GetGameEntity(this EntityComponent entityComponent, int entityId)
        {
            UnityGameFramework.Runtime.Entity entity = entityComponent.GetEntity(entityId);
            if (entity == null)
            {
                return null;
            }

            return (UGFEntity)entity.Logic;
        }

        public static void HideEntity(this EntityComponent entityComponent, UGFEntity ugfEntity)
        {
            entityComponent.HideEntity(ugfEntity.Entity);
        }

        public static void AttachEntity(this EntityComponent entityComponent, UGFEntity ugfEntity, int ownerId, string parentTransformPath = null, object userData = null)
        {
            entityComponent.AttachEntity(ugfEntity.Entity, ownerId, parentTransformPath, userData);
        }

        private static void ShowEntity(this EntityComponent entityComponent, Type logicType, string entityGroup, int priority, UGFEntityData data)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }

            DREntity drEntity = DataTables.Instance.DTEntity.GetOrDefault(data.TypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                return;
            }

            entityComponent.ShowEntity(data.Id, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), entityGroup, priority, data);
        }

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }
    }
}
