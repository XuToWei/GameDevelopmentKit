//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public static class EntityExtension
    {
        public static Entity GetGameEntity(this EntityComponent entityComponent, int entityId)
        {
            UnityGameFramework.Runtime.Entity entity = entityComponent.GetEntity(entityId);
            if (entity == null)
            {
                return null;
            }

            return (Entity)entity.Logic;
        }

        public static void HideEntity(this EntityComponent entityComponent, Entity entity)
        {
            entityComponent.HideEntity(entity.Entity);
        }

        public static void AttachEntity(this EntityComponent entityComponent, Entity entity, int ownerId, string parentTransformPath = null, object userData = null)
        {
            entityComponent.AttachEntity(entity.Entity, ownerId, parentTransformPath, userData);
        }

        public static void ShowMyAircraft(this EntityComponent entityComponent, MyAircraftData data)
        {
            entityComponent.ShowEntity(typeof(MyAircraft), data);
        }

        public static void ShowAircraft(this EntityComponent entityComponent, AircraftData data)
        {
            entityComponent.ShowEntity(typeof(Aircraft), data);
        }

        public static void ShowThruster(this EntityComponent entityComponent, ThrusterData data)
        {
            entityComponent.ShowEntity(typeof(Thruster), data);
        }

        public static void ShowWeapon(this EntityComponent entityComponent, WeaponData data)
        {
            entityComponent.ShowEntity(typeof(Weapon), data);
        }

        public static void ShowArmor(this EntityComponent entityComponent, ArmorData data)
        {
            entityComponent.ShowEntity(typeof(Armor), data);
        }

        public static void ShowBullet(this EntityComponent entityCompoennt, BulletData data)
        {
            entityCompoennt.ShowEntity(typeof(Bullet), data);
        }

        public static void ShowAsteroid(this EntityComponent entityCompoennt, AsteroidData data)
        {
            entityCompoennt.ShowEntity(typeof(Asteroid), data);
        }

        public static void ShowEffect(this EntityComponent entityComponent, EffectData data)
        {
            entityComponent.ShowEntity(typeof(Effect), data);
        }

        private static void ShowEntity(this EntityComponent entityComponent, Type logicType, EntityData data)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }
            
            DREntity drEntity = GameEntry.Tables.DTEntity.GetOrDefault(data.TypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                return;
            }
            
            entityComponent.ShowEntity(data.Id, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), drEntity.EntityGroupName, drEntity.Priority, data);
        }
    }
}
