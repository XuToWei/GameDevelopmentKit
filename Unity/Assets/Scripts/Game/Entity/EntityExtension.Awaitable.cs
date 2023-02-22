using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class EntityExtension
    {
        public static async UniTask<Entity> ShowEntityAsync(this EntityComponent entityComponent, int entityTypeId, Type logicType, object userData = null, CancellationTokenSource cts = null)
        {
            DREntity drEntity = GameEntry.Tables.DTEntity.GetOrDefault(entityTypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", entityTypeId.ToString());
                return null;
            }
            
            return await entityComponent.ShowEntityAsync(entityComponent.GenerateSerialId(), logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), drEntity.EntityGroupName, drEntity.Priority, userData, cts);
        }
        
        public static async UniTask<Entity> ShowEntityAsync<T>(this EntityComponent entityComponent, int entityTypeId, object userData = null, CancellationTokenSource cts = null) where T : EntityLogic
        {
            return await entityComponent.ShowEntityAsync(entityTypeId, typeof (T), userData, cts);
        }
    }
}
