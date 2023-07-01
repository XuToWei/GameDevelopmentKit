using System;
using UnityEngine;

namespace ET
{
    [ChildOf]
    public sealed class UGFEntity : Entity, IAwake<Type, ETMonoEntity>, IDestroy, ILoad
    {
        public UnityGameFramework.Runtime.Entity entity { get; private set; }

        public Type entityEventType { get; private set; }

        public Transform transform { get; private set; }

        public bool isShow => this.m_ETMonoEntity.isShow;

        private ETMonoEntity m_ETMonoEntity;

        [EntitySystem]
        private class UGFEntityAwakeSystem : AwakeSystem<UGFEntity, Type, ETMonoEntity>
        {
            protected override void Awake(UGFEntity self, Type entityEventType, ETMonoEntity etMonoEntity)
            {
                self.m_ETMonoEntity = etMonoEntity;
                self.entityEventType = entityEventType;
                self.transform = etMonoEntity.CachedTransform;
                self.entity = etMonoEntity.Entity;
            }
        }

        [EntitySystem]
        private class UGFEntityDestroySystem : DestroySystem<UGFEntity>
        {
            protected override void Destroy(UGFEntity self)
            {
                self.m_ETMonoEntity = default;
                self.entityEventType = default;
                self.transform = default;
                self.entity = default;
            }
        }

        [EntitySystem]
        private class UGFEntityLoadSystem : LoadSystem<UGFEntity>
        {
            protected override void Load(UGFEntity self)
            {
                self.m_ETMonoEntity.OnLoad();
            }
        }
    }
}