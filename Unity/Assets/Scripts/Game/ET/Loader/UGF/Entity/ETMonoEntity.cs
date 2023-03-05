using System;
using UnityEngine;
using UnityGameFramework.Runtime;
using Game;

namespace ET
{
    [DisallowMultipleComponent]
    public sealed class ETMonoEntity : BaseEntity
    {
        public UGFEntity UGFEntity { get; private set; }
        public Type EntityEventType { get; private set; }
        
        private bool m_IsInitShow = true;
        private IUGFEntityEvent m_EntityEvent;
        
        public void Load()
        {
            if(UGFEventComponent.Instance.TryGetEntityEvent(this.EntityEventType, out IUGFEntityEvent entityEvent))
            {
                this.m_EntityEvent = entityEvent;
            }
            else
            {
                this.m_EntityEvent = default;
                Log.Warning($"EntityEventType {this.EntityEventType} doesn't exist EntityEvent!");
            }
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            this.m_IsInitShow = true;
            ETMonoEntityData entityData = userData as ETMonoEntityData;
            if (entityData == null)
            {
                throw new Exception("ETMonoEntityData is null!");
            }

            if (entityData.ParentEntity == null)
            {
                throw new Exception("ETMonoEntityData ParentEntity is null!");
            }

            this.EntityEventType = entityData.EntityEventType;
            this.UGFEntity = entityData.ParentEntity.AddChild<UGFEntity, Type, ETMonoEntity>(this.EntityEventType, this);

            Load();
            
            this.m_EntityEvent?.OnInit(this.UGFEntity, entityData.UserData);
        }

        private void OnDestroy()
        {
            this.UGFEntity.Dispose();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (this.m_IsInitShow)
            {
                this.m_IsInitShow = false;
                ETMonoEntityData entityData = userData as ETMonoEntityData;
                this.m_EntityEvent?.OnShow(this.UGFEntity, entityData.UserData);
                entityData.Release();
            }
            else
            {
                this.m_EntityEvent?.OnShow(this.UGFEntity, userData);
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            this.m_EntityEvent?.OnHide(this.UGFEntity, isShutdown, userData);
            base.OnHide(isShutdown, userData);
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
            this.m_EntityEvent?.OnAttached(this.UGFEntity, childEntity, parentTransform, userData);
        }

        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
            this.m_EntityEvent?.OnDetached(this.UGFEntity, childEntity, userData);
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            this.m_EntityEvent?.OnAttachTo(this.UGFEntity, parentEntity, parentTransform, userData);
        }

        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            this.m_EntityEvent?.OnDetachFrom(this.UGFEntity, parentEntity, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            this.m_EntityEvent?.OnUpdate(this.UGFEntity, elapseSeconds, realElapseSeconds);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            this.m_EntityEvent?.OnRecycle(this.UGFEntity);
        }
    }
}
