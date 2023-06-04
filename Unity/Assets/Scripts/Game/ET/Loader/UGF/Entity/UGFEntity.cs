using System;
using UnityEngine;

namespace ET
{
    public sealed class UGFEntity : Entity, IAwake<Type, ETMonoEntity>, ILoad
    {
        public ETMonoEntity etMonoEntity
        {
            get;
            private set;
        }

        public Type entityEventType
        {
            get;
            private set;
        }
        
        public Transform transform
        {
            get;
            private set;
        }
        
        public bool isShow;
        
        [EntitySystem]
        private class UGFEntityAwakeSystem: AwakeSystem<UGFEntity, Type, ETMonoEntity>
        {
            protected override void Awake(UGFEntity self, Type entityEventType, ETMonoEntity ugfETUIForm)
            {
                self.etMonoEntity = ugfETUIForm;
                self.entityEventType = entityEventType;
                self.transform = self.etMonoEntity.CachedTransform;
            }
        }
        
        [EntitySystem]
        private class UGFEntityLoadSystem: LoadSystem<UGFEntity>
        {
            protected override void Load(UGFEntity self)
            {
                self.etMonoEntity.OnLoad();
            }
        }
    }
}
