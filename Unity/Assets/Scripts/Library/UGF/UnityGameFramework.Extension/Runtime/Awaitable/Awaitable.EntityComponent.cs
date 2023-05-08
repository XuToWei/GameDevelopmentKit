using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        /// <summary>
        /// 显示实体（可等待）
        /// </summary>
        public static async UniTask<Entity> ShowEntityAsync(this EntityComponent entityComponent, int entityId, Type entityLogicType, string entityAssetName,
            string entityGroupName, int priority = 0, object userData = null, CancellationToken cancellationToken = default)
        {
            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            bool IsFinished()
            {
                return !entityComponent.IsLoadingEntity(entityId);
            }
            await UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                entityComponent.HideEntity(entityId);
                return null;
            }
            return entityComponent.GetEntity(entityId);
        }
    }
}
