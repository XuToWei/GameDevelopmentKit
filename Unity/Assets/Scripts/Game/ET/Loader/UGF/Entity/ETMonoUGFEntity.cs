using UnityEngine;
using Game;
using GameFramework;
using UnityGameFramework.Runtime;

namespace ET
{
    /// <summary>
    /// ET使用的GF的Entity
    /// </summary>
    /// 不包含任何逻辑，ET的Entity持有MonoBehaviour处理
    internal sealed class ETMonoUGFEntity : AEntity
    {
        private UGFEntity ugfEntity;
        private Transform cachedParentTransform;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ETMonoUGFEntityData entityData = (ETMonoUGFEntityData)userData;
            ugfEntity = entityData.UGFEntity;
            ugfEntity.CachedTransform = CachedTransform;
            cachedParentTransform = CachedTransform.parent;
            UGFEntitySystemSingleton.Instance.UGFEntityOnInit(ugfEntity);
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            ETMonoUGFEntityData entityData = (ETMonoUGFEntityData)userData;
            ugfEntity = entityData.UGFEntity;
            ReferencePool.Release(entityData);
            ugfEntity.CachedTransform = CachedTransform;
            UGFEntitySystemSingleton.Instance.UGFEntityOnShow(ugfEntity);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            UGFEntitySystemSingleton.Instance.UGFEntityOnHide(ugfEntity, isShutdown);
            CachedTransform.SetParent(cachedParentTransform, true);
            base.OnHide(isShutdown, userData);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFEntitySystemSingleton.Instance.UGFEntityOnRecycle(ugfEntity);
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)childEntity;
            UGFEntitySystemSingleton.Instance.UGFEntityOnAttached(ugfEntity, entity.ugfEntity, parentTransform);
        }

        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)childEntity;
            UGFEntitySystemSingleton.Instance.UGFEntityOnDetached(ugfEntity, entity.ugfEntity);
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)parentEntity;
            UGFEntitySystemSingleton.Instance.UGFEntityOnAttachTo(ugfEntity, entity.ugfEntity, parentTransform);
        }

        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)parentEntity;
            UGFEntitySystemSingleton.Instance.UGFEntityOnDetachFrom(ugfEntity, entity.ugfEntity);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFEntitySystemSingleton.Instance.UGFEntityOnUpdate(ugfEntity, elapseSeconds, realElapseSeconds);
        }
    }
}
