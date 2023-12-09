using UnityEngine;
using UnityGameFramework.Runtime;
using Game;
using GameFramework;

namespace ET
{
    [DisallowMultipleComponent]
    public sealed class ETMonoEntity : BaseEntity
    {
        private UGFEntity m_UGFEntity;
        private string m_EntityEventTypeName;
        private IUGFEntityEvent m_EntityEvent;

        public bool isShow { get; private set; }
        public UGFEntity ugfEntity => this.m_UGFEntity;

        public void OnReload()
        {
            if(UGFEventComponent.Instance.TryGetEntityEvent(this.m_EntityEventTypeName, out IUGFEntityEvent entityEvent))
            {
                this.m_EntityEvent = entityEvent;
            }
            else
            {
                this.m_EntityEvent = default;
                throw new GameFrameworkException($"EntityEventType {this.m_EntityEventTypeName} doesn't exist EntityEvent!");
            }
            this.m_EntityEvent.OnReload(this.m_UGFEntity);
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            ETMonoEntityData entityData = (ETMonoEntityData)userData;
            if (entityData.ParentEntity == null)
            {
                throw new GameFrameworkException("ETMonoEntityData ParentEntity is null!");
            }
            if (this.m_UGFEntity == default || this.m_EntityEventTypeName != entityData.EntityEventTypeName || entityData.ParentEntity != this.m_UGFEntity.Parent)
            {
                UGFEntityDispose();
                if(UGFEventComponent.Instance.TryGetEntityEvent(entityData.EntityEventTypeName, out IUGFEntityEvent entityEvent))
                {
                    this.m_EntityEvent = entityEvent;
                }
                else
                {
                    this.m_EntityEvent = default;
                    throw new GameFrameworkException($"EntityEventType {this.m_EntityEventTypeName} doesn't exist EntityEvent!");
                }
                this.m_EntityEventTypeName = entityData.EntityEventTypeName;
                this.m_UGFEntity = entityData.ParentEntity.AddChild<UGFEntity, string, ETMonoEntity>(this.m_EntityEventTypeName, this);
                this.m_EntityEvent.OnInit(this.m_UGFEntity, entityData.UserData);
            }
            this.isShow = true;
            this.m_EntityEvent.OnShow(this.m_UGFEntity, entityData.UserData);
            entityData.Release();
        }

        private void UGFEntityDispose()
        {
            if (this.m_UGFEntity != default && !this.m_UGFEntity.IsDisposed)
            {
                UGFEntity ugfEntity = this.m_UGFEntity;
                this.m_UGFEntity = default;
                ugfEntity.Dispose();
            }
        }

        private void OnDestroy()
        {
            UGFEntityDispose();
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            this.m_EntityEvent.OnHide(this.m_UGFEntity, isShutdown, userData);
            this.isShow = false;
            if (isShutdown)
            {
                UGFEntityDispose();
            }
            base.OnHide(isShutdown, userData);
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
            this.m_EntityEvent.OnAttached(this.m_UGFEntity, childEntity, parentTransform, userData);
        }

        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
            this.m_EntityEvent.OnDetached(this.m_UGFEntity, childEntity, userData);
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            this.m_EntityEvent.OnAttachTo(this.m_UGFEntity, parentEntity, parentTransform, userData);
        }

        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            this.m_EntityEvent.OnDetachFrom(this.m_UGFEntity, parentEntity, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            this.m_EntityEvent.OnUpdate(this.m_UGFEntity, elapseSeconds, realElapseSeconds);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
            this.m_EntityEvent.OnRecycle(this.m_UGFEntity);
        }
    }
}
