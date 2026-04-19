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
        private UGFEntity m_UGFEntity;

        public UGFEntity UGFEntity => m_UGFEntity;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            ETMonoUGFEntityData entityData = (ETMonoUGFEntityData)userData;
            m_UGFEntity = entityData.UGFEntity;
            ReferencePool.Release(entityData);
            m_UGFEntity.CachedTransform = CachedTransform;
            m_UGFEntity.UGFMono = this;
            UGFSystemSingleton.Instance.UGFEntityOnShow(m_UGFEntity);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            UGFSystemSingleton.Instance.UGFEntityOnHide(m_UGFEntity, isShutdown);
            base.OnHide(isShutdown, userData);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            UGFSystemSingleton.Instance.UGFEntityOnRecycle(m_UGFEntity);
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)childEntity;
            UGFSystemSingleton.Instance.UGFEntityOnAttached(m_UGFEntity, entity.m_UGFEntity, parentTransform);
        }

        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)childEntity;
            UGFSystemSingleton.Instance.UGFEntityOnDetached(m_UGFEntity, entity.m_UGFEntity);
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)parentEntity;
            UGFSystemSingleton.Instance.UGFEntityOnAttachTo(m_UGFEntity, entity.m_UGFEntity, parentTransform);
        }

        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            ETMonoUGFEntity entity = (ETMonoUGFEntity)parentEntity;
            UGFSystemSingleton.Instance.UGFEntityOnDetachFrom(m_UGFEntity, entity.m_UGFEntity);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            UGFSystemSingleton.Instance.UGFEntityOnUpdate(m_UGFEntity, elapseSeconds, realElapseSeconds);
        }
    }
}
