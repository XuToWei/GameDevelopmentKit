using System;
using UnityEngine;
using UnityGameFramework.Runtime;
using Game;
using GameFramework;

namespace ET
{
    [DisallowMultipleComponent]
    public sealed class ETMonoEntity : BaseEntity
    {
        public UGFEntity ugfEntity { get; private set; }
        public Type entityEventType { get; private set; }
        
        private IUGFEntityEvent m_EntityEvent;
        
        public void OnLoad()
        {
            if(UGFEventComponent.Instance.TryGetEntityEvent(this.entityEventType, out IUGFEntityEvent entityEvent))
            {
                this.m_EntityEvent = entityEvent;
            }
            else
            {
                this.m_EntityEvent = default;
                throw new GameFrameworkException($"EntityEventType {this.entityEventType} doesn't exist EntityEvent!");
            }
            this.m_EntityEvent?.OnLoad(this.ugfEntity);
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            ETMonoEntityData entityData = (ETMonoEntityData)userData;
            if (entityData.ParentEntity == null)
            {
                throw new GameFrameworkException("ETMonoEntityData ParentEntity is null!");
            }
            if(UGFEventComponent.Instance.TryGetEntityEvent(this.entityEventType, out IUGFEntityEvent entityEvent))
            {
                this.m_EntityEvent = entityEvent;
            }
            else
            {
                this.m_EntityEvent = default;
                throw new GameFrameworkException($"EntityEventType {this.entityEventType} doesn't exist EntityEvent!");
            }
            if (this.entityEventType != entityData.EntityEventType)
            {
                this.entityEventType = entityData.EntityEventType;
                this.ugfEntity?.Dispose();
                this.ugfEntity = default;
                this.ugfEntity = entityData.ParentEntity.AddChild<UGFEntity, Type, ETMonoEntity>(this.entityEventType, this);
                this.m_EntityEvent?.OnInit(this.ugfEntity, entityData.UserData);
            }
            this.ugfEntity.isShow = true;
            this.m_EntityEvent?.OnShow(this.ugfEntity, entityData.UserData);
            entityData.Release();
        }

        private void OnDestroy()
        {
            if (this.ugfEntity != default)
            {
                this.ugfEntity.Dispose();
                this.ugfEntity = default;
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            this.m_EntityEvent?.OnHide(this.ugfEntity, isShutdown, userData);
            this.ugfEntity.isShow = false;
            if (isShutdown)
            {
                this.ugfEntity.Dispose();
                this.ugfEntity = default;
            }
            base.OnHide(isShutdown, userData);
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
            this.m_EntityEvent?.OnAttached(this.ugfEntity, childEntity, parentTransform, userData);
        }

        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
            this.m_EntityEvent?.OnDetached(this.ugfEntity, childEntity, userData);
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            this.m_EntityEvent?.OnAttachTo(this.ugfEntity, parentEntity, parentTransform, userData);
        }

        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            this.m_EntityEvent?.OnDetachFrom(this.ugfEntity, parentEntity, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            this.m_EntityEvent?.OnUpdate(this.ugfEntity, elapseSeconds, realElapseSeconds);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            this.m_EntityEvent?.OnRecycle(this.ugfEntity);
        }
    }
}
